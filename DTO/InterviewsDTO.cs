using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.DTO
{
    public class InterviewsDTO
    {
        public int InterviewId { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public DateTime InterviewDate { get; set; }
        public string InterviewType { get; set; }
        public string InterviewName { get; set; }
        public string Interview_status { get; set; }
        public string InterviewTime { get; set; }
        public string InterviewLoc { get; set; }
    }
}
