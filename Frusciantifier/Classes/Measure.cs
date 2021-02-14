using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Frusciantifier.Classes
{
    public class Measure
    {
        public SortedList<NotesKey, Note> Notes { get; private set; }

        private const int MeasureLength = 15;

        public Measure()
        {
            Notes = new SortedList<NotesKey, Note>();
        }


        public Note Add(int column, int row)
        {
            if (GetNotesKey(column, row) != null || !CanNoteBeAddedToRight(column, row))
            {
                return null;
            }

            Note note = new Note(column, row, GetNoteType(column, row));

            Notes.Add(new NotesKey(note.Column, note.Row), note);

            return note;
        }

        private bool CanNoteBeAddedToRight(int column, int row)
        {
            NotesKey notesKeyOnLeft = GetNotesKeyOnleft(column, row);

            if (notesKeyOnLeft == null)
            {
                return true;
            }

            Note noteOnLeft = GetNote(notesKeyOnLeft.Column, notesKeyOnLeft.Row);

            return row >= noteOnLeft.Row + noteOnLeft.Duration - 1;

        }

        private NotesKey GetNotesKeyOnleft(int column, int row)
        {
            return Notes.Where(n => (n.Key as NotesKey).Row == row)
                              .Select(n => n)
                              .Where(n => (n.Key as NotesKey).Column < column)
                              .OrderByDescending(n => (n.Key as NotesKey).Column)
                              .Select(n => n.Key as NotesKey)
                              .FirstOrDefault();
        }

        public void Delete(int column, int row)
        {
            NotesKey note = GetNotesKey(column, row);

            Notes.Remove(note);

        }

        private NotesKey GetNotesKey(int column, int row)
        {

            return Notes.Where(n => (n.Key as NotesKey).Column == column)
                              .Select(n => n)
                              .Where(n => (n.Key as NotesKey).Row == row)
                              .Select(n => n.Key as NotesKey)
                              .FirstOrDefault();
        }

        private NotesKey GetNotesKeys(int[] columns, int row)
        {

            return Notes.Where(n => (n.Key as NotesKey).Column >= columns[0] && (n.Key as NotesKey).Column <= columns[1])
                              .Select(n => n)
                              .Where(n => (n.Key as NotesKey).Row == row)
                              .Select(n => n.Key as NotesKey)
                              .FirstOrDefault();
        }

        public Note GetNote(int column, int row)
        {
            return Notes.Where(n => (n.Key as NotesKey).Column == column)
                              .Select(n => n)
                              .Where(n => (n.Key as NotesKey).Row == row)
                              .Select(n => n.Value as Note)
                              .FirstOrDefault();
        }

        public Canvas HighlightNote(int column, int row)
        {
            Note note = GetNote(column, row);

            if (note == null)
            {
                return null;
            }

            note.SetCanvas(true);

            return note.Canvas;
        }

        public Note UnHighlightNote(int column, int row)
        {
            Note note = GetNote(column, row);

            if (note == null)
            {
                return null;
            }

            note.SetCanvas(false);

            return note;
        }

        private string GetNoteType(int column, int row)
        {
            if (CanNoteBeAddedToLeft(NoteType.one, column, row))
            {
                return NoteType.one_note;
            }

            else if (CanNoteBeAddedToLeft(NoteType.half, column, row))
            {
                return GetNoteName(NoteType.half, row);
            }

            else if (CanNoteBeAddedToLeft(NoteType.quarter, column, row))
            {
                return GetNoteName(NoteType.quarter, row);
            }

            else if (CanNoteBeAddedToLeft(NoteType.eighth, column, row))
            {
                return GetNoteName(NoteType.eighth, row);
            }

            else
            {
                return GetNoteName(NoteType.sixteenth, row);
            }

        }

        private string GetNoteName(string notetype, int row)
        {
            if (row < 10)
            {
                return notetype + "_note_stem_" + "down";
            }

            else
            {
                return notetype + "_note_stem_" + "up";
            }
        }

        private bool CanNoteBeAddedToLeft(string name, int column, int row)
        {
            NotesKey notekey;
            bool isPastMeasure;

            switch (name)
            {
                case NoteType.one:
                    notekey = GetNotesKeys(new int[] { column + 1, column + 15 }, row);
                    isPastMeasure = column + 15 > MeasureLength;
                    return notekey == null && !isPastMeasure;
                case NoteType.half:
                    notekey = GetNotesKeys(new int[] { column + 1, column + 7 }, row);
                    isPastMeasure = column + 7 > MeasureLength;
                    return notekey == null && !isPastMeasure;
                case NoteType.quarter:
                    notekey = GetNotesKeys(new int[] { column + 1, column + 3 }, row);
                    isPastMeasure = column + 3 > MeasureLength;
                    return notekey == null && !isPastMeasure;
                case NoteType.eighth:
                    notekey = GetNotesKeys(new int[] { column + 1, column + 1 }, row);
                    isPastMeasure = column + 1 > MeasureLength;
                    return notekey == null && !isPastMeasure;
                case NoteType.sixteenth:
                    notekey = GetNotesKeys(new int[] { column + 1, column + 0 }, row);
                    isPastMeasure = column + 0 > MeasureLength;
                    return notekey == null && !isPastMeasure;
                default:
                    return false;
            }
        }

        public IList<Note> GetNotes()
        {
            return Notes.Values;
        }

        public Note UpdateNoteDuration(Note note, bool increase)
        {
            string nextNoteType;
            if (increase)
            {
                nextNoteType = CanNoteWithIncreasedDurationBeAddedToLeft(note);
            }

            else
            {
                nextNoteType = GetNoteWithDecreasedDuration(note);
            }

            if (nextNoteType == null)
            {
                return null;
            }

            note.Update(nextNoteType);

            return note;
        }



        private string CanNoteWithIncreasedDurationBeAddedToLeft(Note note)
        {

            string NoteTypeIncreasedDuration = null;

            switch (note.Duration)
            {
                case 1:
                    NoteTypeIncreasedDuration = NoteType.eighth;
                    break;
                case 2:
                    NoteTypeIncreasedDuration = NoteType.quarter;
                    break;
                case 4:
                    NoteTypeIncreasedDuration = NoteType.half;
                    break;
                case 8:
                    NoteTypeIncreasedDuration = NoteType.one;
                    break;
                case 16:
                    NoteTypeIncreasedDuration = null;
                    break;
            }

            if (CanNoteBeAddedToLeft(NoteTypeIncreasedDuration, note.Column, note.Row))
            {
                return NoteTypeIncreasedDuration;
            }
            else
            {
                return null;
            }
        }

        private string GetNoteWithDecreasedDuration(Note note)
        {
            switch (note.Duration)
            {
                case 1:
                    return null;
                case 2:
                    return NoteType.sixteenth;
                case 4:
                    return NoteType.eighth;
                case 8:
                    return NoteType.quarter;
                case 16:
                    return NoteType.half;
                default:
                    return null;
            }
        }

        public Note UpdateNotePosition(Note note, bool toLeft)
        {
            bool canPositionBeAltered;
            if (toLeft)
            {
                canPositionBeAltered = CanNoteBeAddedToRight(note.Column - 1, note.Row);
            }

            else
            {
                canPositionBeAltered = CanNoteBeAddedToLeft(note.Name, note.Column + 1, note.Row);
            }

            if (canPositionBeAltered)
            {
                return note.UpdatePosition(toLeft);
            }


            else
            {
                return null;
            }
        }


    }
}
