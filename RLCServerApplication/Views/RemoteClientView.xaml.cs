﻿using System;
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
using RLCServerApplication.ViewModels;

namespace RLCServerApplication.Views
{
    /// <summary>
    /// Interaction logic for RemoteClientView.xaml
    /// </summary>
    public partial class RemoteClientView : UserControl
    {
        public RemoteClientView()
        {
            InitializeComponent();
        }

        public RemoteClientViewModel RemoteClientViewModel => DataContext as RemoteClientViewModel;
    }
}