using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace klitatOved.Enums
{
    public enum Status
    {
        [Description("חדש")]
        New,

        [Description("בבדיקה")]
        InReview,
        
        [Description("בריאיון")]
        Interview,

        [Description("התקבל")]
        Hired,
        
        [Description("נדחה")]
        Rejected

    }

   
}
