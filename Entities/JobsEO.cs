using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.Entities
{
    [Table("Jobs")]
    public class JobsEO
    {
        [Key]
        [Column("JobId")]
        public int JobId { get; set; }

        [Column("JobTitle")]
        public string JobTitle { get; set; }

        [Column("JobDescription")]
        public string JobDescription { get; set; }

        [Column("JobRequirements")]
        public string JobRequirements { get; set; }

        [Column("Organization")]
        public string Organization { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("Job_status")]
        public string Job_status { get; set; }

        [Column("TotalApplicants")]
        public int TotalApplicants { get; set; }

        [Column("HiredCount")]
        public int HiredCount { get; set; }

        [Column("RejectedCount")]
        public int RejectedCount { get; set; }

        [Column("Fillings_status")]
        public string Fillings_status { get; set; }

        [Column("RequiredPositions")]
        public int RequiredPositions { get; set; }

        //משרה אחת יכולה לקבל כמה מועמדויות וגם להיות מחוברת לכמה ריאיונות
        
        //קשר למועמדויות - אחד לרבים
        public virtual ICollection<JobApplicationsEO> JobGetCandidate { get; set; }

        //קשר לריאיונות - אחד לרבים
        public virtual ICollection<InterviewsEO> jobGetInterview { get; set; }
    }
}
