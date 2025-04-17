using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.Entities
{
    [Table("Candidates")]
    public class CandidatesEO
    {
        [Key]
        [Column("CandidateId")]
        public int CandidateId { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("En_FirstName")]
        public string En_FirstName { get; set; }

        [Column("En_LastName")]
        public string En_LastName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Phone")]
        public string Phone { get; set; }

        [Column("IdNumber")]
        public int IdNumber { get; set; }

        [Column("UserName")]
        public string UserName { get; set; }

        [Column("PasswordHash")]
        public string PasswordHash { get; set; }

        //קשרים - מועמד יכול להגיש כמה מועמדויות וגם לקבל כמה ריאיונות

        //קשר למועמדויות - אחד לרבים
        public virtual ICollection<JobApplicationsEO> CandidatesApplyJob { get; set; }

        //קשר לריאיונות - אחד לרבים
        public virtual ICollection<InterviewsEO> candidtateGetInterview { get; set; }
    }
}
