using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.DTO
{
    public class UpdateJobDTO
    {
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public string JobRequirements { get; set; }
        public string Job_status { get; set; }
        public string Fillings_status { get; set; }
        public int? TotalApplicants { get; set; }
        public int? HiredCount { get; set; }
        public int? RejectedCount { get; set; }
    }
}
