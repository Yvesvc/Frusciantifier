using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frusciantifier.Classes
{
    public class Cell
    {
        public int column;
        public int row;

        public Cell(int c = 0, int r = 0)
        {
            column = c;
            row = r;
        }
    }
}
