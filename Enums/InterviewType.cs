using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace klitatOved.Enums
{
    public enum InterviewType
    {

        [Display(Name = "טלפוני")]
        Phone,

        [Display(Name = "זום")]
        Zoom,

        [Display(Name = "פרונטלי")]
        InPerson

    }

    public static class InterviewHelper
    {
        public static InterviewType TranslateToInterviewType(string interviewType)
        {
            switch (interviewType)
            {
                case "פרונטלי": return InterviewType.InPerson;
                case "זום": return InterviewType.Zoom;
                case "טלפוני": return InterviewType.Phone;
                default: throw new ArgumentException("סוג ריאיון לא תקין");

            }
        }
    }
    
        
}
