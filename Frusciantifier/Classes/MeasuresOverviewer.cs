using Frusciantifier.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Frusciantifier.Classes
{
    //Class no longer used
    public static class MeasuresOverviewer
    {

        public static List<Dictionary<ContentControl, MeasureViewModel>> MeasuresList = new List<Dictionary<ContentControl, MeasureViewModel>>();

        public static ContentControl MostRecentAddedMeasureContentControl
        {
            get; set;
        }

        private static MeasureViewModel _mostRecentAddedMeasureViewModel;
        public static MeasureViewModel MostRecentAddedMeasureViewModel
        {
           get
            {
                return _mostRecentAddedMeasureViewModel;
            }

            set
            {
                _mostRecentAddedMeasureViewModel = value;
                MeasuresList.Add(new Dictionary<ContentControl, MeasureViewModel>(){ { MostRecentAddedMeasureContentControl, MostRecentAddedMeasureViewModel } });
            }
        }
    }
}
