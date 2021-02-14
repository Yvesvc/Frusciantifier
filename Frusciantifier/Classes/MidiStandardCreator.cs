using Frusciantifier.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;


namespace Frusciantifier.Classes
{
    public static class MidiStandardCreator
    {
        public static void Create(Dictionary<ContentControl, MeasureViewModel> measures)
        {
            List<MidiStandardNote> midiStandardNotes = new List<MidiStandardNote>();
            foreach (MeasureViewModel measureViewModel in measures.Values)
            {
                foreach (KeyValuePair<NotesKey, Note> kp in measureViewModel.measure.Notes)
                {
                    Note note = kp.Value;

                    MidiStandardNote midiStandardNote = new MidiStandardNote();
                    midiStandardNote.Duration = (float)note.Duration / 4 ;
                    midiStandardNote.Time = (float)note.Column / 4;
                    midiStandardNote.Degree = RowToDegree(note.Row);

                    midiStandardNotes.Add(midiStandardNote);
                }
            }

            string json = JsonConvert.SerializeObject(midiStandardNotes, Formatting.Indented);

            System.IO.File.WriteAllText(DirectoryWrapper.GetPathCurrentSolution() + "/Song.json", json);
        }

        //TO-DO abstract
        private static int RowToDegree(int row, int degree = 0)
        {
            if (row == 15)
            {
                return degree + 60;
            }

            else if ((15 - row - 3) % 7 == 0 || (15 - row) % 7 == 0)
            {
                return RowToDegree(row + 1, degree += 1);
            }

            else
            {
                return RowToDegree(row + 1, degree += 2);
            }
        }
    }
}
