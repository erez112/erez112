using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace klitatOved.Enums
{
    public enum jobStatus
    {
        Open,
        Closed,
        Canceled
    }
      
    public static class JobsStatuesExtensions
    {
        public static string GetDisplayName(this jobStatus status)
        {
            return status switch
            {
                jobStatus.Open => "פתוחה",
                jobStatus.Closed => "נסגרה",
                jobStatus.Canceled => "בוטלה",
                _ => status.ToString()
            };
        }
    }
         
        
    
}
