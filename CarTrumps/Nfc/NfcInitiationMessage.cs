/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using Windows.Storage.Streams;
using Windows.Networking.Proximity;

namespace CarTrumps.Nfc
{
    /// <summary>
    /// This message type is used in the initiation protocol to decide the
    /// master/slave relationship between the two devices before the game is
    /// started.
    /// </summary>
    public class NfcInitiationMessage
    {
        [DataContract]
        public class InitialMessage
        {
            [DataMember(Name = "devicetime")]
            public double devicetime;
        }
        
        private InitialMessage initialMessage = new InitialMessage();
        private ProximityDevice proximityDevice;
        private NotifyNfcReady notifyReady;
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private NfcManager.ProtoState state = NfcManager.ProtoState.NotReady;
        private long subscribeId = -1;
        private long publishId = -1;
        private bool isMaster = false;

        public delegate void NotifyNfcReady();

        public bool IsMaster()
        {
            return isMaster;
        }

        public bool IsReady()
        {
            return (state == NfcManager.ProtoState.Ready);
        }

        public void PingAdversary(ProximityDevice device, NotifyNfcReady notify)
        {
            if (subscribeId != -1)
            {
                proximityDevice.StopSubscribingForMessage(subscribeId);
                subscribeId = -1;
            }
            
            if (publishId != -1)
            {
                proximityDevice.StopPublishingMessage(publishId);
                publishId = -1;
            }

            if (state == NfcManager.ProtoState.Busy)
            {
                return;
            }

            state = NfcManager.ProtoState.NotReady;
            notifyReady = notify;
            initialMessage.devicetime = random.NextDouble();
            MemoryStream stream = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(initialMessage.GetType());
            serializer.WriteObject(stream, initialMessage);
            stream.Position = 0;
            var dataWriter = new DataWriter();
            dataWriter.WriteBytes(stream.GetBuffer());
            proximityDevice = device;
            publishId = proximityDevice.PublishBinaryMessage("Windows.CarTrumps", dataWriter.DetachBuffer());
            subscribeId = proximityDevice.SubscribeForMessage("Windows.CarTrumps", OnMessageReceived);
       }

        private void OnMessageReceived(ProximityDevice sender, ProximityMessage message)
        {
            if (state == NfcManager.ProtoState.Ready)
            {
                return;
            }
            
            state = NfcManager.ProtoState.NotReady;

            try
            {
                String msg = message.DataAsString.Substring(0, message.DataAsString.IndexOf('\0'));
                MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(msg));
                DataContractSerializer serializer = new DataContractSerializer(typeof(InitialMessage));
                stream.Position = 0;
                InitialMessage adversaryTime = (InitialMessage)serializer.ReadObject(stream);
                isMaster = (initialMessage.devicetime > adversaryTime.devicetime);
                state = NfcManager.ProtoState.Ready;
                proximityDevice.StopSubscribingForMessage(subscribeId);
                proximityDevice.StopPublishingMessage(publishId);
                subscribeId = -1;
                publishId = -1;
                notifyReady();
            }
            catch (SerializationException)
            {
            }
        }
    }
}
