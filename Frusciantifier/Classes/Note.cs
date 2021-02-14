using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Frusciantifier.Classes
{
    public class Note
    {
        private int _row;
        public int Row
        {
            get
            {
                return _row;
            }

            set
            {
                _row = value;
                OnNoteChanged();
            }
        }

        public int _column;
        public int Column
        {
            get
            {
                return _column;
            }

            set
            {
                _column = value;
                OnNoteChanged();
            }
        }


        public int DisplayColumn { get; set; }
        public int DisplayRow { get; set; }

        public int Length { get; set; }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                SetDuration();
                OnNoteChanged();
            }
        }

        public Canvas Canvas { get; set; }

        public int Duration { get; set; }

        private event EventHandler NoteChanged;

        public Note(int column, int row, string name)
        {
            NoteChanged += SetDisplayColumnAndRow;
            NoteChanged += SetCanvasTag;

            SetName(name, row);
            

            Row = row;
            Column = column;

            SetCanvas(false);
        }


        private void SetName(string name, int row)
        {
            if (name == "")
            {
                if (row < 10)
                {
                    Name = NoteType.eighth_note_stem_down;
                }
                else
                {
                    Name = NoteType.eighth_note_stem_up;
                }
            }

            else
            {
                Name = name;
            };
        }

        public void SetDisplayColumnAndRow([CallerMemberName] object sender = null, EventArgs e = null)
        {

            if (Name.Equals(NoteType.eighth_note_stem_up) || Name.Equals(NoteType.quarter_note_stem_up) || Name.Equals(NoteType.half_note_stem_up) || Name.Equals(NoteType.sixteenth_note_stem_up))
            {
                DisplayColumn = Column;
                DisplayRow = Row - 7;
            }

            else if (Name.Equals(NoteType.half_rest))
            {
                DisplayColumn = Column;
                DisplayRow = 3;
            }

            else if (Name.Equals(NoteType.quarter_rest))
            {
                DisplayColumn = Column;
                DisplayRow = 7;
            }

            else
            {
                DisplayColumn = Column;
                DisplayRow = Row;
            }
        }

        public void SetCanvas(bool isSelected = false)
        {
            XmlReader xmlNote;

            if (isSelected == false)
            {
                xmlNote = XmlReader.Create(DirectoryWrapper.GetPathCurrentSolution() + "/Frusciantifier/Notes/Standard/" + this.Name + ".xml");
            }
            else
            {
                xmlNote = XmlReader.Create(DirectoryWrapper.GetPathCurrentSolution() + "/Frusciantifier/Notes/Highlighted/" + this.Name + ".xml");
            }

           Canvas = (Canvas)XamlReader.Load(xmlNote);
           Canvas.Tag = new Cell(this.Column, this.Row);

        }

        private void SetDuration()
        {
            if (Name == NoteType.one_note)
            {
                Duration =  16;
            }

            else if (Name == NoteType.half_note_stem_down || Name == NoteType.half_note_stem_up)
            {
                Duration = 8;
            }

            else if (Name == NoteType.quarter_note_stem_down || Name == NoteType.quarter_note_stem_up)
            {
                Duration = 4;
            }
            else if (Name == NoteType.eighth_note_stem_down || Name == NoteType.eighth_note_stem_up)
            {
                Duration = 2;
            }

            else if (Name == NoteType.sixteenth_note_stem_down || Name == NoteType.sixteenth_note_stem_up)
            {
                Duration = 1;
            }

            else
            {
                Duration = 0;
            }
        }


        private void OnNoteChanged([CallerMemberName] object sender = null)
        {
            NoteChanged(this, EventArgs.Empty);
        }

        public Note DeepCopy()
        {
            Note newNote = (Note)this.MemberwiseClone();
            return newNote;
        }

        public void Update(string noteType)
        {
            string name = null;
            if (Row < 10)
            {
                switch (noteType)
                {
                    case NoteType.one:
                        name = NoteType.one_note;
                        break;
                    case NoteType.half:
                        name = NoteType.half_note_stem_down;
                        break;
                    case NoteType.quarter:
                        name = NoteType.quarter_note_stem_down;
                        break;
                    case NoteType.eighth:
                        name = NoteType.eighth_note_stem_down;
                        break;
                    case NoteType.sixteenth:
                        name = NoteType.sixteenth_note_stem_down;
                        break;
                }
            }
            else
            {
                switch (noteType)
                {
                    case NoteType.one:
                        name = NoteType.one_note;
                        break;
                    case NoteType.half:
                        name = NoteType.half_note_stem_up;
                        break;
                    case NoteType.quarter:
                        name = NoteType.quarter_note_stem_up;
                        break;
                    case NoteType.eighth:
                        name = NoteType.eighth_note_stem_up;
                        break;
                    case NoteType.sixteenth:
                        name = NoteType.sixteenth_note_stem_up;
                        break;
                }
            }

            SetName(name, Row);
            SetCanvas(true);
        }

        public Note UpdatePosition(bool toLeft)
        {
            if(toLeft)
            {
                Column -= 1;
                return this;
            }

            else
            {
                Column += 1;
                return this;
            }
        }

        private void SetCanvasTag(object sender, EventArgs e)
        {
            if (Canvas != null)
            {
                Canvas.Tag = new Cell(this.Column, this.Row);
            }
            
        }

    }
}
