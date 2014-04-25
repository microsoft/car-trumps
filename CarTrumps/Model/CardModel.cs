/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;
using CarTrumps.Nfc;

namespace CarTrumps.Model
{
    /// <summary>
    /// Container for the cards in the deck. This class also implements some
    /// methods for managing the game logic including like, for instance,
    /// comparison between a property in a card.
    /// </summary>
    public class CardModel : ObservableCollection<Card>
    {
        private Random _random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 1000);

        private Card _activeCard;
        public Card ActiveCard
        {
            get
            {
                return _activeCard;
            }
            set
            {
                _activeCard = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ActiveCard"));
            }
        }

        private Card _challengerCard;
        public Card ChallengerCard
        {
            get
            {
                return _challengerCard;
            }
            set
            {
                _challengerCard = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ChallengerCard"));
            }
        }

        private string _statusMsg;
        public string StatusMsg
        {
            get
            {
                return _statusMsg;
            }
            set
            {
                _statusMsg = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("StatusMsg"));
            }
        }

        private string _selectedCardPropertyName;
        public string SelectedCardPropertyName
        {
            get
            {
                return _selectedCardPropertyName;
            }
            set
            {
                _selectedCardPropertyName = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedCardPropertyName"));
            }
        }

        private float _ownValue = 0;
        public float OwnValue
        {
            get
            {
                return _ownValue;
            }
            set
            {
                _ownValue = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("OwnValue"));
            }
        }

        private float _opponentValue = 0;
        public float OpponentValue
        {
            get
            {
                return _opponentValue;
            }
            set
            {
                _opponentValue = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("OpponentValue"));
            }
        }

        private int visibleCardNum = 1;

        private int _cardLeft = 0;
        public int CardLeft
        {
            get
            {
                return _cardLeft;
            }
            set
            {
                _cardLeft = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CardLeft"));
            }
        }

        private Visibility _showUltimateWinner = Visibility.Collapsed;
        public Visibility ShowUltimateWinner
        {
            get
            {
                return _showUltimateWinner;
            }
            set
            {
                _showUltimateWinner = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ShowUltimateWinner"));
            }
        }

        private Visibility _showUltimateLooser = Visibility.Collapsed;
        public Visibility ShowUltimateLooser
        {
            get
            {
                return _showUltimateLooser;
            }
            set
            {
                _showUltimateLooser = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ShowUltimateLooser"));
            }
        }

        public enum ComparisonResult
        {
            EWeWon,
            EWeLost,
            ETie
        }

        public ComparisonResult Results
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CardModel()
        {
        }

        /// <summary>
        /// Populates the model based on the contents of carddata.xml.
        /// </summary>
        public void CreateCards()
        {
            XDocument xdoc = XDocument.Load("content/carddata.xml");
            XElement cardsElement = xdoc.Element("cards");

            IEnumerable<XElement> cards = cardsElement.Descendants("card");
            foreach (var cardItem in cards)
            {
                Card card = new Card();
                card.CardId = GetInt(cardItem.Element("id").Value);
                card.Name = cardItem.Element("name").Value;
                card.ImagePath = cardItem.Element("image").Value;
                card.Power = GetInt(cardItem.Element("power").Value);
                card.Engine = GetInt(cardItem.Element("engine").Value);
                card.Torque = GetInt(cardItem.Element("torque").Value);
                card.Speed = GetInt(cardItem.Element("speed").Value);
                card.Economy = GetFloat(cardItem.Element("economy").Value);
                card.Acceleration = GetFloat(cardItem.Element("acceleration").Value);
                card.IsEnabled = true;
                card.IsWinner = Visibility.Collapsed;
                this.Add(card);
            }

            // Set first card as active card
            ActiveCard = this[0];
            ChallengerCard = null;
        }

        /// <summary>
        /// Select the next card from hand.
        /// </summary>
        public void NextCard()
        {
            NextCardRecurse();
            visibleCardNum++;

            if (visibleCardNum > EnabledCardCount())
            {
                visibleCardNum = 1;
            }

            ActiveCard.CardOrderNum = visibleCardNum;
        }

        private void NextCardRecurse()
        {
            if (ActiveCard != null)
                ActiveCard.IsWinner = Visibility.Collapsed;

            if (EnabledCardCount() <= 1)
            {
                SelectFirstEnabledCard();
                return;
            }

            int index = this.IndexOf(ActiveCard);
            
            if (index + 1 <= this.Count - 1)
            {
                ActiveCard = this[index + 1];
            }
            else
            {
                ActiveCard = this[0];
            }
            
            ActiveCard.IsWinner = Visibility.Collapsed;

            if (ActiveCard.IsEnabled == false && EnabledCardCount() > 1)
            {
                NextCardRecurse();
            }
        }

        /// <summary>
        /// Select the previous card from hand.
        /// </summary>
        public void PreviousCard()
        {
            PreviousCardRecurse();
            visibleCardNum--;

            if (visibleCardNum == 0)
            {
                visibleCardNum = EnabledCardCount();
            }

            ActiveCard.CardOrderNum = visibleCardNum;
        }

        private void PreviousCardRecurse()
        {
            if (EnabledCardCount() <= 1)
            {
                SelectFirstEnabledCard();
                return;
            }

            int index = this.IndexOf(ActiveCard);
            
            if (index - 1 >= 0)
            {
                ActiveCard = this[index - 1];
            }
            else
            {
                ActiveCard = this[this.Count - 1];
            }

            if (ActiveCard.IsEnabled == false && EnabledCardCount() > 1)
            {
                PreviousCardRecurse();
            }
        }

        /// <summary>
        /// Count the active cards in our hand.
        /// </summary>
        public int EnabledCardCount()
        {
            int count = 0;
            
            foreach (Card card in this)
            {
                if (card.IsEnabled)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Enable/disable all cards from the deck.
        /// </summary>
        public void EnableAllCards(bool enable)
        {
            foreach (Card card in this)
            {
                card.IsEnabled = enable;
            }
        }

        /// <summary>
        /// Set the active card to first card in our hand.
        /// </summary>
        public void SelectFirstEnabledCard()
        {
            foreach (Card card in this)
            {
                if (card.IsEnabled)
                {
                    ActiveCard = card;
                    break;
                }
            }

            App.CardModel.CardLeft = App.CardModel.EnabledCardCount();
            ActiveCard.CardOrderNum = visibleCardNum;
        }

        /// <summary>
        /// Randomize and split the deck into two hands.
        /// </summary>
        public int[] SuffleCards()
        {
            int[] challengerCards = new int[this.Count / 2];
            int cnt = 0;

            EnableAllCards(true);

            while (cnt < this.Count / 2)
            {
                int index = _random.Next(this.Count - 1);

                if (this[index].IsEnabled == true)
                {
                    challengerCards[cnt] = index;
                    this[index].IsEnabled = false;
                    cnt++;
                }
            }

            SelectFirstEnabledCard();
            return challengerCards;
        }

        public void UseCards(Message msg)
        {
            if (msg != null)
            {
                // Use opponent's deck
                EnableAllCards(false);

                foreach (int index in msg.CardIds)
                {
                    this[index].IsEnabled = true;
                }
            }

            if (ActiveCard == null || !ActiveCard.IsEnabled)
            {
                SelectFirstEnabledCard();
            }
        }

        /// <summary>
        /// Decide which card is the winner.
        /// </summary>
        public bool SelectWinnerCard()
        {
            if (SelectedCardPropertyName == null || ChallengerCard == null)
            {
                return false;
            }

            float ownCard = ActiveCard.PropertyFloatValue(SelectedCardPropertyName);
            float opponentCard = ChallengerCard.PropertyFloatValue(SelectedCardPropertyName);
            OwnValue = ownCard;
            OpponentValue = opponentCard;

            // Highest value wins, except when comparing the economy or acceleration
            if (SelectedCardPropertyName == "Economy" ||
                SelectedCardPropertyName == "Acceleration")
            {
                ownCard = -ownCard;
                opponentCard = -opponentCard;
            }

            ActiveCard.IsWinner = Visibility.Collapsed;
            ChallengerCard.IsWinner = Visibility.Collapsed;

            if (ownCard > opponentCard)
            {
                // We won, enable the challenger's card
                Results = ComparisonResult.EWeWon;
                ChallengerCard.IsEnabled = true;
                if (App.CardModel.CardLeft == Count - 1)
                {
                    //show "game over" , ultimate winner
                    ShowUltimateWinner = Visibility.Visible;
                }
                else
                {
                    ActiveCard.IsWinner = Visibility.Visible;
                }

                InitCardListing();
            }
            else if (ownCard < opponentCard)
            {
                // We lost, disable our active card
                Results = ComparisonResult.EWeLost;
                ActiveCard.IsEnabled = false;

                if (App.CardModel.CardLeft == 1)
                {
                    // Show "game over", ultimate looser
                    ShowUltimateLooser = Visibility.Visible;
                }
                else
                {
                    ChallengerCard.IsWinner = Visibility.Visible;
                }

                InitCardListing();
            }
            else
            {
                // It's a tie
                Results = ComparisonResult.ETie;
            }

            return (Results == ComparisonResult.EWeWon);
        }

        public bool IsNumeric(object expression)
        {
            if (expression == null)
            {
                return false;
            }

            float number;
            return float.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }

        public float GetFloat(object expression)
        {
            float ret = 0;
            
            if (IsNumeric(expression))
            {
                float.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture),
                    System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out ret);
            }

            return ret;
        }

        public int GetInt(object expression)
        {
            int ret = 0;
            
            if (IsNumeric(expression))
            {
                int.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture),
                    System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out ret);
            }

            return ret;
        }

        public void InitCardListing()
        {
            visibleCardNum = 1;
            ActiveCard.CardOrderNum = visibleCardNum;
        }
    }
}
