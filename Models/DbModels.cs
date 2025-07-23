using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace work_01_MD.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        [Required, StringLength(50), Display(Name = "Skill Name")]
        public string SkillName { get; set; } = default!;
        //nev
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    }
    public class Candidate
    {
        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required, StringLength(50)]
        public string Phone { get; set; } = default!;
        public string Image { get; set; } = default!;
        public bool Fresher { get; set; }
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    }
    public class CandidateSkill
    {
        public int CandidateSkillId { get; set; }
        public int SkillId { get; set; }
        public int CandidateId { get; set; }
        //nev
        public virtual Skill Skill { get; set; } = default!;
        public virtual Candidate Candidate { get; set; } = default!;
    }
    public class CandidateDbContext : DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options) : base(options)
        {

        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }

        public void InsertSkill(Skill sk)
        {
            SqlParameter skName = new SqlParameter("@skName", sk.SkillName);
            this.Database.ExecuteSqlRaw("EXEC spInsertSkill @skName", skName);
        }

    }
}
