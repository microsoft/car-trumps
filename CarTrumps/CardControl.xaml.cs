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
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CarTrumps
{
    public partial class GameCard : UserControl
    {
        private Border currSelection = null;
        private bool isSelectionOn = true;

        public GameCard()
        {
            InitializeComponent();
        }

        public void ClearSelection()
        {
            if (currSelection != null)
                currSelection.Opacity = 0;
        }

        public void AllowSelection(bool val)
        {
            isSelectionOn = val;
        }

        /// <summary>
        /// Called when a border in the page is tapped. Each card property in the page is inside a border.
        /// </summary>
        private void border_Tap(object sender, GestureEventArgs e)
        {
            if (!isSelectionOn)
                return;

            if(currSelection != null)
                currSelection.Opacity = 0;

            currSelection = sender as Border;

            // Get selected property name
            App.CardModel.SelectedCardPropertyName = (string)currSelection.GetType().GetProperty("Name").GetValue(currSelection);

            // Publish the selected card and property with NFC
            App.NfcManager.ShowCard();
            App.CardModel.StatusMsg = "Tap the phones to compare the cards";

            currSelection.Opacity = 0.3;
        }
    }
}
