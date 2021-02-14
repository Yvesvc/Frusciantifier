using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xml;

namespace Frusciantifier.Views
{

    public partial class MeasureView : Window
    {
        public MeasureView()
        {
            InitializeComponent();
        }


        private void MusicGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)e.Source;

            int c = Grid.GetColumn(element);
            int r = Grid.GetRow(element);

        }
    }

}