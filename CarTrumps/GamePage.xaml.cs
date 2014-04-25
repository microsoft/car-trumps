/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using CarTrumps.Nfc;

namespace CarTrumps
{
    public partial class GamePage : PhoneApplicationPage
    {
        private int prevIndex;
        private bool allowSelection = false;

        public GamePage()
        {
            InitializeComponent();
            this.DataContext = App.CardModel;
            App.NfcManager.MessageCardReceived += MessageReceived;
            pivot.SelectionChanged += pivot_SelectionChanged;
            prevIndex = pivot.SelectedIndex;
        }

        void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool directionRight = false;
            int lastIdx = pivot.Items.Count - 1;

            if (pivot.SelectedIndex == 0 && prevIndex == lastIdx) // going right over the last item
            {
                directionRight = true;
            }
            else if (pivot.SelectedIndex == lastIdx && prevIndex == 0) // going left over the first item
            {
                directionRight = false;
            }
            else
            {
                directionRight = (prevIndex < pivot.SelectedIndex);  // normal listing
            }

            if (directionRight)           
            {
                App.CardModel.NextCard();
            }
            else
            {
                App.CardModel.PreviousCard();
            }

            prevIndex = pivot.SelectedIndex;
            ClearSelection();
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (WinnerPage.gotoInitPage)
            {
                WinnerPage.gotoInitPage = false;
                NavigationService.GoBack();
            }

            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey(MainPage.paramkey))
                allowSelection = (Int32.Parse(parameters[MainPage.paramkey]) == 1);
            
            App.CardModel.SelectFirstEnabledCard(); 
            ClearSelection();
        }

        /// <summary>
        /// Clears the active card property selection.
        /// </summary>
        public void ClearSelection()
        {
            App.CardModel.ActiveCard.IsWinner = Visibility.Collapsed;
            if(App.CardModel.ChallengerCard != null)
                App.CardModel.ChallengerCard.IsWinner = Visibility.Collapsed;

            //clear property selection
            if (pivot.SelectedItem != null)
            {
                GameCard card = ((pivot.SelectedItem as PivotItem).Content as GameCard);
                card.ClearSelection();
                card.AllowSelection(allowSelection);
            }

            // Enable menu bar buttons.
            ApplicationBar.IsVisible = true;
            App.CardModel.SelectedCardPropertyName = null;

            // Publish the selected card and property with NFC
            App.NfcManager.ShowCard();
        }

        /// <summary>
        /// NFC message event handler.
        /// </summary>
        public void MessageReceived(Message msg)
        {
            // GamePage handles only the showcard -message
            if (msg == null || msg.MessageType != Message.TypeEnum.EShowCard)
            {
                return;
            }

            bool needRepublish = false;
            // Both players must select the same property, e.g. "engine"
            if (msg.SelectedCardProperty != App.CardModel.SelectedCardPropertyName)
            {
                if (App.CardModel.SelectedCardPropertyName == null && msg.SelectedCardProperty != null)
                {
                    App.CardModel.SelectedCardPropertyName = msg.SelectedCardProperty;
                }
                else if (App.CardModel.SelectedCardPropertyName != null && msg.SelectedCardProperty == null)
                {
                    msg.SelectedCardProperty = App.CardModel.SelectedCardPropertyName;
                }
                else
                {
                    App.CardModel.StatusMsg = "Your opponent selected " + msg.SelectedCardProperty;
                    needRepublish = true;
                }
            }                    
            else if (App.CardModel.SelectedCardPropertyName == null && msg.SelectedCardProperty == null)
            {
                needRepublish = true;
            }

            if (needRepublish)
            {
                ClearSelection();
            }
            else
            {
                // Find the card and decide which one wins
                foreach (CarTrumps.Model.Card card in App.CardModel)
                {
                    if (card.CardId == msg.CardId)
                    {
                        App.CardModel.ChallengerCard = card;
                        App.CardModel.SelectWinnerCard();
                        // Show the winner -control and disable the menu bar buttons
                        NavigationService.Navigate(new Uri("/WinnerPage.xaml", UriKind.Relative));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Previous card button clicked
        /// </summary>
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            ClearSelection();
            App.CardModel.PreviousCard();
        }

        /// <summary>
        /// Next card button clicked
        /// </summary>
        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            ClearSelection();
            App.CardModel.NextCard();
        }
    }
}
