using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.DTO
{
    public class CandidatesDTO
    {
        public int CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string En_FirstName { get; set; }
        public string En_LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdNumber { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

    }
}
