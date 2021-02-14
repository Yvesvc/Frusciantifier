using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Frusciantifier.Classes
{
    public static class DirectoryWrapper
    {
        public static string GetPathCurrentSolution()
        {
            return Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        }
    }
}
