/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarTrumps.Model
{
    /// <summary>
    /// Represents a single card in the deck.
    /// </summary>
    [DataContract]
    public class Card : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Card()
        {
        }

        public string PropertyValue(string propertyName)
        {
            return this.GetType().GetProperty(propertyName).GetValue(this).ToString();
        }

        public float PropertyFloatValue(string propertyName)
        {
            object val1 = this.GetType().GetProperty(App.CardModel.SelectedCardPropertyName).GetValue(this);
            return App.CardModel.GetFloat(val1);
        }

        [DataMember]
        public int CardId
        {
            get
            {
                return _cardId;
            }
            set 
            {
                if (_cardId != value)
                {
                    _cardId = value;
                    NotifyPropertyChanged("CardId");
                }
            }
        }
        int _cardId;

        [DataMember]
        public int CardOrderNum
        {
            get {
                return _cardOrderNum;
            }
            set 
            {
                if (_cardOrderNum != value)
                {
                    _cardOrderNum = value;
                    NotifyPropertyChanged("CardOrderNum");
                }
            }
        }
        int _cardOrderNum;

        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        string _name;

        [DataMember]
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    NotifyPropertyChanged("ImagePath");
                }
            }
        }
        string _imagePath;

        [DataMember]
        public int Power
        {
            get
            {
                return _power;
            }
            set
            {
                if (_power != value)
                {
                    _power = value;
                    NotifyPropertyChanged("Power");
                }
            }
        }
        int _power;

        [DataMember]
        public int Torque
        {
            get
            {
                return _torque;
            }
            set
            {
                if (_torque != value)
                {
                    _torque = value;
                    NotifyPropertyChanged("Torque");
                }
            }
        }
        int _torque;

        [DataMember]
        public float Acceleration
        {
            get
            {
                return _acceleration;
            }
            set
            {
                if (_acceleration != value)
                {
                    _acceleration = value;
                    NotifyPropertyChanged("Acceleration");
                }
            }
        }
        float _acceleration;

        [DataMember]
        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }
        int _speed;

        [DataMember]
        public int Engine
        {
            get
            {
                return _engine;
            }
            set
            {
                if (_engine != value)
                {
                    _engine = value;
                    NotifyPropertyChanged("Engine");
                }
            }
        }
        int _engine;

        [DataMember]
        public float Economy
        {
            get
            {
                return _economy;
            }
            set
            {
                if (_economy != value)
                {
                    _economy = value;
                    NotifyPropertyChanged("Economy");
                }
            }
        }
        float _economy;

        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged("IsEnabled");
                }
            }
        }
        bool _isEnabled;

        [DataMember]
        public Visibility IsWinner
        {
            get
            {
                return _isWinner;
            }
            set
            {
                if (_isWinner != value)
                {
                    _isWinner = value;
                    NotifyPropertyChanged("IsWinner");
                }
            }
        }
        Visibility _isWinner;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
