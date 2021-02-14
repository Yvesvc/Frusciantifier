using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using Frusciantifier.Classes;
using Frusciantifier.Views;
using Prism.Commands;

namespace Frusciantifier.ViewModels
{
    public class MeasureViewModel
    {
        private Grid measureGrid;
        public Measure measure;

        private Dictionary<string, int> _selectedCell = new Dictionary<string, int>();
        Dictionary<string, int> SelectedCell
        {
            get
            {
                return _selectedCell;
            }
            set
            {
                if (SelectedCell.Column() != PreviousSelectedCell.Column() || SelectedCell.Row() != PreviousSelectedCell.Row())
                {
                    PreviousSelectedCell = new Dictionary<string, int>(SelectedCell);
                }
                _selectedCell = value;
            }
        }

        private Dictionary<string, int> _previousSelectedCell = new Dictionary<string, int>();
        Dictionary<string, int> PreviousSelectedCell
        {
            get
            {
                return _previousSelectedCell;
            }
            set
            {
                _previousSelectedCell = value;
            }
        }

        private Note _selectedNote;

        public Note SelectedNote
        {
            get
            {
                return _selectedNote;
            }

            set
            {
                if (SelectedNote != null && (PreviousSelectedNote == null || SelectedNote.Column != PreviousSelectedNote.Column || SelectedNote.Row != PreviousSelectedNote.Row))
                {
                    PreviousSelectedNote = SelectedNote.DeepCopy();
                }
                _selectedNote = value;
            }
        }

        private Note _previousSelectedNote;
        public Note PreviousSelectedNote
        {
            get
            {
                return _previousSelectedNote;
            }

            set
            {
                _previousSelectedNote = value;
            }
        }


        private Note _mostRecentAddedNote;
        public Note MostRecentAddedNote
        {
            get
            {
                return _mostRecentAddedNote;
            }
            set
            {
                PreviousMostRecentAddedNote = MostRecentAddedNote?.DeepCopy();
                _mostRecentAddedNote = value;
            }
        }

        Note PreviousMostRecentAddedNote { get; set; }


        public ICommand LoadedCommand { get; set; }
        public ICommand EditNoteCommand { get; set; }
        public ICommand DeleteNoteCommand { get; set; }
        public ICommand UpdateNoteDurationWithMouseCommand { get; set; }
        public ICommand UpdateNoteWithKeyCommand { get; set; }


        public MeasureViewModel()
        {

            MainViewModel.MostRecentAddedMeasureViewModel = this;

            LoadedCommand = new DelegateCommand<RoutedEventArgs>(OnGridLoaded);
            EditNoteCommand = new DelegateCommand<MouseButtonEventArgs>(OnMeasureGridLeftClicked, CanOnMeasureGridLeftClicked);
            DeleteNoteCommand = new DelegateCommand<MouseButtonEventArgs>(OnMeasureGridRightClicked);
            UpdateNoteDurationWithMouseCommand = new DelegateCommand<MouseWheelEventArgs>(OnMeasureGridScrolled);
            UpdateNoteWithKeyCommand = new DelegateCommand<KeyEventArgs>(OnMeasureGridKeyDown);
            measure = new Measure();

            PreviousSelectedCell = new Dictionary<string, int> { { "column", -1 }, { "row", -1 } };
            SelectedCell["column"] = 0;
            SelectedCell["row"] = 0;

        }



        private void OnGridLoaded(RoutedEventArgs e)
        {
            measureGrid = (Grid)e.Source;
            measureGrid.Focusable = true;
        }

        public void OnMeasureGridLeftClicked(MouseButtonEventArgs e)
        {

            UpdateSelectedColumnAndRow(e.Source as FrameworkElement);

            if (!IsNotePresentOnSelectedCell())
            {
                AddNote();
            }

            UpdateHighlightedNote();
        }

        private bool CanOnMeasureGridLeftClicked(MouseButtonEventArgs e)
        {
            int row = GetSelectedRow(e.Source as FrameworkElement);
            return  2 <= row && row <= 15 ;
        }

        private int GetSelectedRow(FrameworkElement source)
        {
            if (source.Parent is Grid)
            {
                return (int)source.GetValue(Grid.RowProperty);
            }

            else
            {
                return GetSelectedRow(source.Parent as FrameworkElement);
            }
        }

        private bool IsNotePresentOnSelectedCell()
        {
            return measure.GetNote(SelectedCell.Column(), SelectedCell.Row()) != null;
        }

        private void UpdateSelectedColumnAndRow(FrameworkElement source)
        {
            if (source.Parent is Grid)
            {
                if (source.GetType() == typeof(Canvas))
                {
                    SelectedCell = new Dictionary<string, int> { { "column", ((source as Canvas).Tag as Cell).column }, { "row", ((source as Canvas).Tag as Cell).row } };
                }
                else
                {
                    SelectedCell = new Dictionary<string, int> { { "column", (int)source.GetValue(Grid.ColumnProperty) }, { "row", (int)source.GetValue(Grid.RowProperty) } };
                }


                Note note = measure.GetNote(SelectedCell.Column(), SelectedCell.Row());

                if (note != null)
                {
                    SelectedNote = note;
                }
            }

            else
            {
                //testing purposes
                int col = (int)source.GetValue(Grid.ColumnProperty);
                int row = (int)source.GetValue(Grid.RowProperty);


                UpdateSelectedColumnAndRow(source.Parent as FrameworkElement);
            }
        }

        private void AddNote()
        {

            Note note = measure.Add(SelectedCell.Column(), SelectedCell.Row());

            if (note != null)
            {
                SelectedNote = note;
                MostRecentAddedNote = note;
            }

            AddNoteToXAML(note);
        }

        private void AddNoteToXAML(Note note)
        {
            if (note == null)
            {
                return;
            }

            measureGrid.AddInGrid(note);

        }

        private void UpdateHighlightedNote()
        {
            if (measure.GetNote(SelectedCell.Column(), SelectedCell.Row()) == null)
            {
                HighlightNote(MostRecentAddedNote);
            }

            else
            {
                Note selectedNote = measure.GetNote(SelectedCell.Column(), SelectedCell.Row());

                if (selectedNote == null)
                {
                    return;
                }

                HighlightNote(selectedNote);

            }

            UnHighlightNote();
        }

        private void HighlightNote(Note note)
        {
            measureGrid.Children.Remove(note.Canvas);
            note.Canvas = measure.HighlightNote(note.Column, note.Row);
            measureGrid.AddInGrid(note);
        }


        private void UnHighlightNote()
        {
            if (PreviousSelectedNote == null || (SelectedCell.Column() == PreviousSelectedNote.Column && SelectedCell.Row() == PreviousSelectedNote.Row))
            {
                return;
            }

            measureGrid.Children.Remove(PreviousSelectedNote.Canvas);
            PreviousSelectedNote = measure.UnHighlightNote(PreviousSelectedNote.Column, PreviousSelectedNote.Row);
            measureGrid.AddInGrid(PreviousSelectedNote);
        }

        public void UnHighlightAllNotes()
        {
            foreach (Note note in measure.GetNotes())
            {
                measureGrid.Children.Remove(note.Canvas);
                measureGrid.AddInGrid(measure.UnHighlightNote(note.Column, note.Row));
            }


        }

        private void OnMeasureGridRightClicked(MouseButtonEventArgs e)
        {
            UpdateSelectedColumnAndRow(e.Source as FrameworkElement);

            Note note = measure.GetNote(SelectedCell.Column(), SelectedCell.Row());

            if (note != null)
            {

                UnHighlightNote();

                measure.Delete(note.Column, note.Row);
                measureGrid.Children.Remove(note.Canvas);

                SelectedNote = null;
                PreviousSelectedNote = null;
            }

        }

        private void OnMeasureGridScrolled(MouseWheelEventArgs e)
        {
            if (SelectedNote == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                UpdateNoteDuration(SelectedNote, true);
            }

            else
            {
                UpdateNoteDuration(SelectedNote, false);
            }
        }


        private void OnMeasureGridKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                UpdateNoteDuration(SelectedNote, false);
            }

            else if (e.Key == Key.Up)
            {
                UpdateNoteDuration(SelectedNote, true);
            }

            else if (e.Key == Key.Left)
            {
                UpdateNotePosition(SelectedNote, true);
            }

            else if (e.Key == Key.Right)
            {

            }
        }


        private void UpdateNoteDuration(Note SelectedNote, bool increase)
        {
            Canvas canvas = SelectedNote.Canvas;
            Note note = measure.UpdateNoteDuration(SelectedNote, increase);
            if (note == null)
            {
                return;
            }
            measureGrid.Children.Remove(canvas);
            AddNoteToXAML(note);

        }

        private void UpdateNotePosition(Note note, bool toLeft)
        {
            Canvas canvas = SelectedNote.Canvas;
            note = measure.UpdateNotePosition(SelectedNote, toLeft);
            if (note == null)
            {
                return;
            }
            measureGrid.Children.Remove(canvas);
            AddNoteToXAML(note);
        }

        private Canvas GetSelectedNote(FrameworkElement source)
        {
            if (source == null)
            {
                return null;
            }

            else if (source.Parent is Canvas && (source.Parent as Canvas).Parent is Grid)
            {
                return source.Parent as Canvas;
            }

            else
            {
                return GetSelectedNote(source.Parent as FrameworkElement);
            }
        }

        public void FocusMeasureGrid()
        {
            measureGrid.Focus();
        }

    }
}
