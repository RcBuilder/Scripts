using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCNUtils.enums
{
    public enum eTransactionType
    {
        S = 0,  // Sales  - Regular
        L = 1,  //        - unidentified sale     
        M = 2,
        Y = 3,
        I = 4,
        T = 5,   // Input - Regular
        K = 6,
        R = 7,
        P = 8,
        H = 9,
        C = 10
    }
}
