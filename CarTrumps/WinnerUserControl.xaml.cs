/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CarTrumps
{
    public partial class WinnerUserControl : UserControl
    {
        public WinnerUserControl()
        {
            InitializeComponent();
            DataContext = App.CardModel;
        }
    }
}
