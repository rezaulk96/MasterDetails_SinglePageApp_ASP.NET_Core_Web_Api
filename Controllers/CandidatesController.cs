using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using work_01_MD.Models;
using work_01_MD.Models.ViewModels;

namespace work_01_MD.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly CandidateDbContext _context;
        private readonly IWebHostEnvironment _he;
        public CandidatesController(CandidateDbContext _context, IWebHostEnvironment _he)
        {
            this._context = _context;
            this._he = _he;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Candidates.Include(x => x.CandidateSkills).ThenInclude(y => y.Skill).ToListAsync());
        }

        public IActionResult Aggregation()
        {
            ViewBag.count = _context.Candidates.Count();

            return View();
        }
        public IActionResult AddNewSkills(int? id)
        {
            ViewBag.skills = new SelectList(_context.Skills, "SkillId", "SkillName", id.ToString() ?? "");
            return PartialView("_AddNewSkills");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CandidateVM candidateVM, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateName = candidateVM.CandidateName,
                    DateOfBirth = candidateVM.DateOfBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher
                };

                //file
                var file = candidateVM.ImageFile;
                string webroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(candidateVM.ImageFile.FileName);
                string fileSave = Path.Combine(webroot, folder, imgFileName);

                if (file != null)
                {
                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        candidateVM.ImageFile.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }

                //for skill
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        Candidate = candidate,
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    _context.CandidateSkills.Add(candidateSkill);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(x => x.CandidateId == id);
            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                DateOfBirth = candidate.DateOfBirth,
                Phone = candidate.Phone,
                Image = candidate.Image,
                Fresher = candidate.Fresher
            };

            //skill
            var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach (var item in existSkill)
            {
                candidateVM.SkillList.Add(item.SkillId);
            }
            return View(candidateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CandidateVM candidateVM, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateId = candidateVM.CandidateId,
                    CandidateName = candidateVM.CandidateName,
                    DateOfBirth = candidateVM.DateOfBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher
                };

                var file = candidateVM.ImageFile;
                var oldPic = candidateVM.Image;
                if (file != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Path.GetFileName(candidateVM.ImageFile.FileName);
                    string fileSave = Path.Combine(webroot, folder, imgFileName);
                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        candidateVM.ImageFile.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                else
                {
                    candidate.Image = oldPic;
                }
                //skill
                var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                foreach (var item in existSkill)
                {
                    _context.CandidateSkills.Remove(item);
                }

                //add
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    _context.CandidateSkills.Add(candidateSkill);
                }
                _context.Update(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(x => x.CandidateId == id);
            var existSkill = _context.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach (var item in existSkill)
            {
                _context.CandidateSkills.Remove(item);
            }
            _context.Remove(candidate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
