using Frusciantifier.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Frusciantifier.Classes
{
    public static class ExtensionClass
    {
        public static void AddInGrid (this Grid grid, Note note)
        {
            grid.Children.Add(note.Canvas);
            Grid.SetColumn(note.Canvas, note.DisplayColumn); //note.Canvas.SetValue(Grid.ColumnProperty, note.DisplayColumn);
            Grid.SetRow(note.Canvas, note.DisplayRow); // note.Canvas.SetValue(Grid.RowProperty, note.DisplayRow);
        }

        public static void AddInGrid (this Grid grid, ContentControl contentControl, int newMeasureColumn, int newMeasureRow)
        {
            grid.Children.Add(contentControl);
            Grid.SetColumn(contentControl, newMeasureColumn); 
            Grid.SetRow(contentControl, newMeasureRow); 
        }

        public static int Column(this Dictionary<string,int> dictionary)
        {
            return dictionary["column"];
        }

        public static int Row(this Dictionary<string,int> dictionary)
        {
            return dictionary["row"];
        }

        public static KeyValuePair<ContentControl, MeasureViewModel> GetMeasureContentControlViewModelPair(this Dictionary<ContentControl, MeasureViewModel> dic, ContentControl contentControl)
        {
            foreach(KeyValuePair<ContentControl, MeasureViewModel> pair in dic)
            {
                if (pair.Key == contentControl)
                {
                    return pair;
                }
            }
            return default(KeyValuePair<ContentControl, MeasureViewModel>);
        }

    }
}
