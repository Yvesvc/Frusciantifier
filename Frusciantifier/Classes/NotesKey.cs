using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frusciantifier.Classes
{
    public class NotesKey : IComparable
    {

        public int Column;

        public int Row;

        public NotesKey(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int CompareTo(object obj)
        {
            int result;

            NotesKey otherNotesKey = obj as NotesKey;

            if (Column == otherNotesKey.Column)
            {
                result = Row < otherNotesKey.Row ? -1 : 1;
            }
            
            else
            {
                result = Column < otherNotesKey.Column ? -1 : 1;
            }

            return result;
            
        }
    }
}
