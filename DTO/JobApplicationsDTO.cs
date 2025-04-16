using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.DTO
{
    public class JobApplicationsDTO
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ResumePath { get; set; }
        public string Progress_Status { get; set; }
    }
}
