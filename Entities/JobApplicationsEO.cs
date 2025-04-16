using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.Entities
{
    [Table("JobApplications")]

    public class JobApplicationsEO
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [ForeignKey("CandidateId")]
        [Column("CandidateId")]
        public int CandidateId { get; set; }
        public virtual CandidatesEO Candidate { get; set; }

        [ForeignKey("JobId")]
        [Column("JobId")]
        public int JobId { get; set; }
        public virtual JobsEO Job { get; set; }

        [Column("ApplicationDate")]
        public DateTime ApplicationDate { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("ResumePath")]
        public string ResumePath { get; set; }

        [Column("Progress_Status")]
        public string Progress_Status { get; set; }

    }
}
