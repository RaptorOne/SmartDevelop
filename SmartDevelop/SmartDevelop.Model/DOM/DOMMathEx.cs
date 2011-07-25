using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevelop.Model.DOM
{
    public static class DOMMathEx
    {
        public static int ShiftLeft ( int x, int shift ) 
        { return x << shift; }

        public static int ShiftRigth(int x, int shift) 
        { return x >> shift; }
    }
}
