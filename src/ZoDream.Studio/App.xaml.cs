using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Studio.ViewModels;

namespace ZoDream.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            ViewModel = new();
        }

        internal static MainViewModel? ViewModel { get; set; }
    }
}
