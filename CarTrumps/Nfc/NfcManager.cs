/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.Proximity;

namespace CarTrumps.Nfc
{
    /// <summary>
    /// The main interface for NFC application logic.
    /// </summary>
    public class NfcManager
    {
        /// <summary>
        /// Definitions for the protocol states.
        /// </summary>
        public enum ProtoState
        {
            NotReady,
            Ready, // Used by NfcInitiationMessage
            Busy, // Used by NfcInitiationMessage
            DealCard,
            ShowCard
        };

        private NfcMessage _nfcMessage = new NfcMessage();
        private NfcInitiationMessage _nfcInitMsg = new NfcInitiationMessage();

        private ProximityDevice _proximityDevice = null;
		private long _publishedMsgId = -1;
        private long _subscribedMsgId = -1;

        public delegate void MessageReceivedHandler(Message msg);
        public event MessageReceivedHandler MessageReceived;
        public event MessageReceivedHandler MessageCardReceived;

        private int[] opponentsCards = null;

        private ProtoState state = ProtoState.NotReady;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NfcManager()
        {
            // Use and initialize the default proximity device.
            // "NFC not supported" exception should not occur, since the appmanifest
            // contains ID_REQ_NFC (installation should not be possible to a phone
            // without NFC).
            _proximityDevice = ProximityDevice.GetDefault();
            
            if (_proximityDevice == null)
            {
                throw new NotSupportedException("NFC not supported");
            }

            // Subscrive for arrive/departed events. These are not needed in
            // this app, but they may prove helpful when debugging.
            _proximityDevice.DeviceArrived += _proximityDevice_DeviceArrived;
            _proximityDevice.DeviceDeparted += _proximityDevice_DeviceDeparted;
        }

        /// <summary>
        /// Occurs when a device leaves the proximate range.
        /// </summary>
        void _proximityDevice_DeviceDeparted(ProximityDevice sender)
        {
            // Not used
        }

        /// <summary>
        /// Occurs when a device enters the proximate range.
        /// </summary>
        void _proximityDevice_DeviceArrived(ProximityDevice sender)
        {
            // Not used
        }

        /// <summary>
        /// Stops all, publishing and subscribing.
        /// </summary>
        public void StopAll()
        {
            if (_publishedMsgId != -1)
            {
                _proximityDevice.StopPublishingMessage(_publishedMsgId);
                _publishedMsgId = -1;
            }

            if (_subscribedMsgId != -1)
            {
                _proximityDevice.StopSubscribingForMessage(_subscribedMsgId);
                _subscribedMsgId = -1;
            }
        }

        public void ProtocolInit(NfcInitiationMessage.NotifyNfcReady notify)
        {
		    StopAll();
            state = ProtoState.NotReady;
            _nfcInitMsg.PingAdversary(_proximityDevice, notify);
        }

        public bool IsMaster()
        {
            return _nfcInitMsg.IsMaster();
        }

        /// <summary>
        /// Publish and subscribe a dealcards -message.
        /// </summary>
        public void DealCards()
        {
            state = ProtoState.DealCard;

            if (IsMaster())
            {
                opponentsCards = App.CardModel.SuffleCards();
                Message msg = new Message(Message.TypeEnum.EDealCards);
                msg.CardIds = opponentsCards;

                // Construct and serialize a dealcards -message.
                MemoryStream mstream = _nfcMessage.SerializeMessage(msg);

                var dataWriter = new Windows.Storage.Streams.DataWriter();
                dataWriter.WriteBytes(mstream.GetBuffer());


                // Publish the message
                _publishedMsgId = _proximityDevice.PublishBinaryMessage("Windows.CarTrumps",
                    dataWriter.DetachBuffer(), NfcWriteCallback);
            }
            else
            {
                // subscribe for a reply
                _subscribedMsgId = _proximityDevice.SubscribeForMessage("Windows.CarTrumps", 
                    NfcMessageReceived);
            }
        }

        /// <summary>
        /// Publish and subscribe a showcard -message.
        /// </summary>
        public void ShowCard()
        {
            state = ProtoState.ShowCard;

            StopAll();

            // Construct and serialize a showcard -message.
            Message msg = new Message(Message.TypeEnum.EShowCard);
            msg.CardId = (ushort)App.CardModel.ActiveCard.CardId;
            msg.SelectedCardProperty = App.CardModel.SelectedCardPropertyName;
            MemoryStream mstream = _nfcMessage.SerializeMessage(msg);

            var dataWriter = new Windows.Storage.Streams.DataWriter();
            dataWriter.WriteBytes(mstream.GetBuffer());

            // Publish the message
            _publishedMsgId = _proximityDevice.PublishBinaryMessage("Windows.CarTrumps",
                dataWriter.DetachBuffer(), NfcWriteCallback);
            // and subscribe for a reply
            _subscribedMsgId = _proximityDevice.SubscribeForMessage("Windows.CarTrumps",
                NfcMessageReceived);
        }

        /// <summary>
        /// The handler that the proximity provider will call when it delivers a message.
        /// </summary>
        private void NfcWriteCallback(ProximityDevice sender, long messageId)
        {
            if (state != ProtoState.DealCard)
                return;

            // Invoke the UI thread since NFC works in separate thread(s)
            Deployment.Current.Dispatcher.BeginInvoke(() => {
                this.MessageReceived(null); // this is the signal to trigger the game start, it conveys just dummy data
            });
        }

        /// <summary>
        /// The handler that the proximity provider will call when it receives a message.
        /// </summary>
        private void NfcMessageReceived(ProximityDevice sender, ProximityMessage message)
        {
            Message msg = _nfcMessage.DeserializeMessage(message.Data);

            // We got the message, stop subscribing for more
            _proximityDevice.StopSubscribingForMessage(_subscribedMsgId);
            _subscribedMsgId = -1;

            // Invoke the UI thread since NFC works in separate thread(s)
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                switch (state)
                {
                    case ProtoState.DealCard:
                        this.MessageReceived(msg);
                        break;
                    case ProtoState.ShowCard:
                        this.MessageCardReceived(msg);
                        break;
                }
            });
        }
    }
}
