using Frusciantifier.Classes;
using Frusciantifier.UserControls;
using Frusciantifier.Views;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Media;
using WMPLib;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Frusciantifier.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public ICommand MeasuresGridLoadedCommand { get; set; }
        public ICommand AddMeasureCommand { get; set; }
        public ICommand RemoveMeasureCommand { get; set; }
        public ICommand UpdateHighlightedMeasureCommand { get; set; }
        public ICommand PlayCommand { get; set; }

        Grid measuresGrid;

        int lastAddedMeasureColumn;
        int lastAddedMeasureRow;

        public List<ContentControl> measuresContentControl = new List<ContentControl>();

        public static Dictionary<ContentControl, MeasureViewModel> Measures = new Dictionary<ContentControl, MeasureViewModel>();

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
                Measures.Add(MostRecentAddedMeasureContentControl, MostRecentAddedMeasureViewModel);
            }
        }


        private KeyValuePair<ContentControl, MeasureViewModel> _activeMeasureContentControlViewModelPair;
        public KeyValuePair<ContentControl, MeasureViewModel> ActiveMeasureContentControlViewModelPair
        {
            get
            {
                return _activeMeasureContentControlViewModelPair;
            }
            set
            {
                if (_activeMeasureContentControlViewModelPair.Key == value.Key && _activeMeasureContentControlViewModelPair.Value == value.Value)
                {
                    return;
                }
                _activeMeasureContentControlViewModelPair = value;
                UnHighlightInActiveMeasures();
                _activeMeasureContentControlViewModelPair.Value.FocusMeasureGrid();

            }
        }

        private event EventHandler ActiveMeasureContentControlViewModelPairChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _playSource;
        public string PlaySource
        {
            get
            {
                return _playSource;
            }

            set
            {
                _playSource = value;
                NotifyPropertyChanged();
            }

        }

        private bool isPlaying { get; set; }

        public MainViewModel()
        {
            PlaySource = "/Icons/play.png";

            MeasuresGridLoadedCommand = new DelegateCommand<RoutedEventArgs>(OnMeasuresGridLoaded);
            UpdateHighlightedMeasureCommand = new DelegateCommand<RoutedEventArgs>(OnMeasuresGridClicked);
            AddMeasureCommand = new DelegateCommand<RoutedEventArgs>(OnAddMeasureLeftButtonUp);
            RemoveMeasureCommand = new DelegateCommand<RoutedEventArgs>(OnRemoveMeasureLeftButtonUp);
            PlayCommand = new DelegateCommand<RoutedEventArgs>(OnPlayLeftButtonUp, CanOnPlayLeftButtonUp);

        }

        private void OnMeasuresGridLoaded(RoutedEventArgs e)
        {
            measuresGrid = (Grid)e.Source;

            AddMeasureContentControl(true);
        }

        private void OnAddMeasureLeftButtonUp(RoutedEventArgs obj)
        {
            AddMeasureContentControl();
        }

        private void OnRemoveMeasureLeftButtonUp(RoutedEventArgs obj)
        {
            if (measuresContentControl.Count == 1)
            {
                return;
            }

            measuresContentControl.Remove(MostRecentAddedMeasureContentControl);


            lastAddedMeasureColumn = lastAddedMeasureColumn == 0 ? measuresGrid.ColumnDefinitions.Count - 1 : lastAddedMeasureColumn - 1;

            if (lastAddedMeasureColumn == measuresGrid.ColumnDefinitions.Count - 1)
            {
                lastAddedMeasureRow -= 1;
            }


            measuresGrid.Children.Remove(MostRecentAddedMeasureContentControl);

            MostRecentAddedMeasureContentControl = measuresContentControl.Last();
        }

        private bool CanOnPlayLeftButtonUp(RoutedEventArgs arg)
        {
            return !isPlaying;
        }

        private void OnPlayLeftButtonUp(RoutedEventArgs obj)
        {
            PlaySong();
        }

        private void PlaySong()
        {
            isPlaying = true;
            PlaySource = "/Icons/playing.png";

            MidiStandardCreator.Create(Measures);

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            process.Exited += PlayMidi;
            process.StartInfo.FileName = DirectoryWrapper.GetPathCurrentSolution() + "/CreateMidi.bat";
            process.Start();
        }

        private void PlayMidi(object sender, EventArgs e)
        {
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            wmp.URL = DirectoryWrapper.GetPathCurrentSolution() + "/finalmidi.mid";
            wmp.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
        }


        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPPlayState)NewState == WMPPlayState.wmppsMediaEnded || (WMPPlayState)NewState == WMPPlayState.wmppsStopped)
            {
                PlaySource = "/Icons/play.png";
                isPlaying = false;
            }
        }


        private void AddMeasureContentControl(bool setup = false)
        {
            ContentControl measureContentControl = new ContentControl();
            measuresContentControl.Add(measureContentControl);

            MostRecentAddedMeasureContentControl = measureContentControl;

            AddTemplateToMeasureContentControl(measureContentControl);


            int newMeasureColumn = lastAddedMeasureColumn == measuresGrid.ColumnDefinitions.Count - 1 || setup ? 0 : lastAddedMeasureColumn + 1;
            int newMeasureRow = lastAddedMeasureColumn == measuresGrid.ColumnDefinitions.Count - 1 ? lastAddedMeasureRow + 1 : lastAddedMeasureRow;

            measuresGrid.AddInGrid(measureContentControl, newMeasureColumn, newMeasureRow);

            lastAddedMeasureColumn = newMeasureColumn;
            lastAddedMeasureRow = newMeasureRow;
        }

        private void AddTemplateToMeasureContentControl(ContentControl measureContentControl)
        {
            ControlTemplate controlTemplate = new ControlTemplate(typeof(ContentControl));
            FrameworkElementFactory measureControl = new FrameworkElementFactory(typeof(MeasureControl));
            controlTemplate.VisualTree = measureControl;

            measureContentControl.Template = controlTemplate;

        }

        private void OnMeasuresGridClicked(RoutedEventArgs e)
        {
            ActiveMeasureContentControlViewModelPair = Measures.GetMeasureContentControlViewModelPair((ContentControl)e.Source);
        }

        private void UnHighlightInActiveMeasures()
        {
            foreach (KeyValuePair<ContentControl, MeasureViewModel> measureContentControlViewModelPair in Measures)
            {
                if (measureContentControlViewModelPair.Key == ActiveMeasureContentControlViewModelPair.Key && measureContentControlViewModelPair.Value == ActiveMeasureContentControlViewModelPair.Value)
                {
                    continue;
                }

                else
                {
                    measureContentControlViewModelPair.Value.UnHighlightAllNotes();
                }
            }
        }

    }


}
