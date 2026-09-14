using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Enums
{
    // this enum is used to represent the status of an application in the system
    // it is stored as an integer in the database
    public enum ApplicationStatus
    {
        Submitted,       // 0 
        Approved,        // 1
        Rejected,        // 2
        Draft            // 3
    }
}
