using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.Entities
{
    [Table("Interviews")]
    public class InterviewsEO
    {
        [Key]
        [Column("InterviewId")]
        public int InterviewId { get; set; }

        [ForeignKey("CandidateId")]
        [Column("CandidateId")]
        public int CandidateId { get; set; }
        public virtual CandidatesEO Candidate { get; set; }

        [ForeignKey("JobId")]
        [Column("JobId")]
        public int JobId { get; set; }
        public virtual JobsEO Job { get; set; }

        [Column("InterviewDate")]
        public DateTime InterviewDate { get; set; }

        [Column("InterviewType")]
        public string InterviewType { get; set; }

        [Column("InterviewName")]
        public string InterviewName { get; set; }

        [Column("Interview_status")]
        public string Interview_status { get; set; }

        [Column("InterviewTime")]
        public string InterviewTime { get; set; }

        [Column("InterviewLoc")]
        public string InterviewLoc { get; set; }

    }
}
