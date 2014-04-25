/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CarTrumps.Nfc;

namespace CarTrumps
{
    public partial class MainPage : PhoneApplicationPage
    {
        public const String paramkey = "allowSelection";

        public MainPage()
        {
            InitializeComponent();

            App.CardModel.CreateCards();
            this.DataContext = App.CardModel;

            App.NfcManager.MessageReceived += MessageReceived;
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.NfcManager.ProtocolInit(NotifyProtocolInit);
        }

        public void NotifyProtocolInit()
        {
            //NOTE: this is called in NFC tread
            this.Dispatcher.BeginInvoke(delegate { 
                //TODO: removeme -- debugging indicator below
                //ismaster.Visibility = (App.NfcManager.IsMaster()) ? Visibility.Visible : Visibility.Collapsed;
                App.NfcManager.DealCards();
            });
        }

        /// <summary>
        /// NFC message event handler.
        /// </summary>
        public void MessageReceived(Message msg)
        {
            //  MainPage handles the deal cards -message, which starts a new game.
            // Both devices publish a set of cards, UseCards decides which
            // one to use based on the random id (uniqueid).
            App.CardModel.UseCards(msg);
            App.CardModel.InitCardListing();
            NavigationService.Navigate(new Uri("/GamePage.xaml?" + paramkey + "=1", UriKind.Relative));
        }

        /// <summary>
        /// Info -button click handler.
        /// </summary>
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/InfoPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Browse cards -button click handler.
        /// </summary>
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            App.CardModel.EnableAllCards(true);
            App.CardModel.CardLeft = App.CardModel.EnabledCardCount();
            App.NfcManager.StopAll();
            App.CardModel.StatusMsg = "";
            App.CardModel.InitCardListing();
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }
    }
}
