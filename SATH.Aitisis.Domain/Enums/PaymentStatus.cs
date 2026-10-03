using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Enums
{
    // the enums are beeing stored as integers in the DB
    public enum PaymentStatus
    {
        Pending,        // 0 
        Completed,      // 1
        Failed          // 2
    }
}
