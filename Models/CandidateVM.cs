using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace work_01_MD.Models.ViewModels
{
    public class CandidateVM
    {

        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required, StringLength(50)]
        public string Phone { get; set; } = default!;
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; } = default!;
        public bool Fresher { get; set; }
        public List<int> SkillList { get; set; } = new List<int>();

    }
}
