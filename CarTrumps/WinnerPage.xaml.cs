/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CarTrumps
{
    public partial class WinnerPage : PhoneApplicationPage
    {
        public static bool gotoInitPage = false;

        public WinnerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            crown.Visibility = App.CardModel.ShowUltimateWinner;
            winnerText.Visibility = App.CardModel.ShowUltimateWinner;
            loosersText.Visibility = App.CardModel.ShowUltimateLooser;
            gameover.Visibility = (App.CardModel.ShowUltimateLooser == Visibility.Collapsed &&
                App.CardModel.ShowUltimateWinner == Visibility.Collapsed) ? Visibility.Collapsed : Visibility.Visible;

            if (gotoInitPage)
            {
                NavigationService.GoBack();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (App.CardModel.ShowUltimateWinner == Visibility.Visible ||
                App.CardModel.ShowUltimateLooser == Visibility.Visible)
            {
                App.CardModel.ShowUltimateWinner = Visibility.Collapsed;
                App.CardModel.ShowUltimateLooser = Visibility.Collapsed;
                gotoInitPage = true;
            }
        }
    }
}