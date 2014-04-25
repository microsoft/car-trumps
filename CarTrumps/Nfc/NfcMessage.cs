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

namespace CarTrumps.Nfc
{
    /// <summary>
    /// Container for the application specific NFC message content.
    /// </summary>
    public class NfcMessage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NfcMessage()
        {
        }

        /// <summary>
        /// Serializes this message into a stream (XML).
        /// </summary>
        public MemoryStream SerializeMessage(Message message)
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                stream.Position = 0;
                DataContractSerializer serializer = new DataContractSerializer(typeof(Message));
                serializer.WriteObject(stream, message);
            }
            catch (SerializationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return stream;
        }

        /// <summary>
        /// Construct this message from a stream (XML received via NFC).
        /// </summary>
        public Message DeserializeMessage(IBuffer buffer)
        {
            Message message = null;

            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Message));
                DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
                byte[] bytes = new byte[buffer.Length];
                dataReader.ReadBytes(bytes);
                System.Diagnostics.Debug.WriteLine(System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                MemoryStream stream = new MemoryStream(bytes, 0, bytes.Length);
                message = (Message)serializer.ReadObject(stream);
            }
            catch (SerializationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return message;
        }
    }

    [DataContract]
    public class Message
    {
        public enum TypeEnum
        {
            EDealCards,
            EShowCard
        }
        
        [DataMember]
        public TypeEnum MessageType
        {
            get;
            set;
        }

        /// <summary>
        /// Used in EDealCards message. Random Card IDs from the deck (the hand).
        /// </summary>
        [DataMember]
        public int[] CardIds
        {
            get;
            set;
        }

        /// <summary>
        /// Used in EShowCard message. The selected card id.
        /// </summary>
        [DataMember]
        public int CardId
        {
            get;
            set;
        }

        /// <summary>
        /// Used in EShowCard message. The selected card property.
        /// </summary>
        [DataMember]
        public string SelectedCardProperty
        {
            get;
            set;
        }

        public Message(TypeEnum type)
        {
            MessageType = type;
        }
    }
}
