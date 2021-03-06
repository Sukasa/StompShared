﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using STOMP;

namespace STOMP.Frames
{
    /// <summary>
    ///     Base class for all Stomp protocol frames
    /// </summary>
    /// <remarks>
    ///     The StompFrame class represents the base class for all Stomp frames.  It contains some basic headers and the list of any non-standard headers to include with any frame
    /// </remarks>
    public abstract class StompFrame
    {
        [StompHeaderIdentifier("receipt", true)]
        internal string _Receipt;

        [StompHeaderIdentifier("transaction", true)]
        internal string _TransactionId;

        /// <summary>
        ///     The receipt Id you would like the server to send back upon successful processing of the frame
        /// </summary>
        public string Receipt
        {
            get
            {
                return _Receipt;
            }
            set
            {
                _Receipt = value;
            }
        }

        /// <summary>
        ///     The Id of the transaction this frame is to be considered a part of, if it is part of a transaction
        /// </summary>
        public string TransactionId
        {
            get
            {
                return _TransactionId;
            }
            set
            {
                _TransactionId = value;
            }
        }

        /// <summary>
        ///     Any additional headers to be included in the packet that are not part of the base specification
        /// </summary>
        public readonly List<KeyValuePair<string, string>> AdditionalHeaders = new List<KeyValuePair<string, string>>();

        public byte[] Serialize()
        {
            StringBuilder Packet = new StringBuilder();
            Type FrameType = this.GetType();
            StompFrameType SFT = FrameType.GetCustomAttribute<StompFrameType>();
            if (SFT == null)
                throw new InvalidOperationException("Attempt to serialize frame without frame type attribute");

            Packet.Append(SFT._FrameType.ToUpper());
            Packet.Append("\n");

            foreach (MemberInfo MI in FrameType.FindMembers(MemberTypes.Field | MemberTypes.Property,
                                                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                            HeaderSearchFilter, null))
            {
                StompHeaderIdentifier HI = MI.GetCustomAttribute<StompHeaderIdentifier>();
                object ValueRaw = null;
                String Value = null;

                switch (MI.MemberType)
                {
                    case MemberTypes.Property:
                        ValueRaw = ((PropertyInfo)MI).GetValue(this);
                        break;
                    case MemberTypes.Field:
                        ValueRaw = ((FieldInfo)MI).GetValue(this);
                        break;
                }

                if (ValueRaw == null || String.IsNullOrWhiteSpace(Value = ValueRaw.ToString()))
                {
                    if (!HI._IsOptional)
                        throw new InvalidOperationException(String.Format("Mandatory parameter {0} is null", MI.Name));
                }
                else
                {
                    Packet.Append(HI._HeaderIdentifier);
                    Packet.Append(":");
                    Packet.Append(Value);
                    Packet.Append("\n");
                }
            }

            foreach (KeyValuePair<string, string> KVP in AdditionalHeaders)
            {
                Packet.Append(KVP.Key);
                Packet.Append(":");
                Packet.Append(KVP.Value);
                Packet.Append("\n");
            }

            if (this is StompBodiedFrame && ((StompBodiedFrame)this).BodyData != null)
            {
                Packet.Append("\n");
                byte[] PacketData = Encoding.UTF8.GetBytes(Packet.ToString());
                byte[] FrameData = new byte[PacketData.Length + ((StompBodiedFrame)this).BodyData.Length];

                Array.Copy(PacketData, 0, FrameData, 0, PacketData.Length);
                Array.Copy(((StompBodiedFrame)this).BodyData, 0, FrameData, PacketData.Length, ((StompBodiedFrame)this).BodyData.Length);

                return FrameData;
            }
            else
            {
                return Encoding.UTF8.GetBytes(Packet.ToString());
            }
        }

        internal static bool HeaderSearchFilter(MemberInfo mi, object Compare)
        {
            StompHeaderIdentifier ID = mi.GetCustomAttribute<StompHeaderIdentifier>();
            return ID != null && ((Compare is string && ID._HeaderIdentifier == (string)Compare) || Compare == null);
        }

        public static StompFrame Build(byte[] Packet, IDictionary<string, Type> TypeDictionary)
        {
            string PacketAsString = Encoding.UTF8.GetString(Packet);
            StompStringReader Reader = new StompStringReader(PacketAsString);


            int Read = 0;
            string PacketType = Reader.ReadUntil('\r', '\n').ToUpper();
            Reader.SkipThrough('\r', '\n');

            if (!TypeDictionary.ContainsKey(PacketType))
                throw new InvalidOperationException("Server sent unrecognized packet type");

            Type FrameType = TypeDictionary[PacketType.ToUpper()];
            StompFrame Frame = (StompFrame)Activator.CreateInstance(FrameType, true);

            // Assign header values here
            do
            {
                string Header = Reader.ReadUntil(':');
                Reader.Shuttle(1);
                string Value = Reader.ReadUntil('\r', '\n');

                MemberInfo MI = null;
                MemberInfo[] MIs = FrameType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                      new MemberFilter(HeaderSearchFilter), Header);
                if (MIs.Length > 0)
                    MI = MIs[0];

                // If in mapping, set property
                if (MI != null)
                {
                    switch (MI.MemberType)
                    {
                        case MemberTypes.Field:
                            ((FieldInfo)MI).SetValue(Frame, Value);
                            break;
                        case MemberTypes.Property:
                            ((PropertyInfo)MI).SetValue(Frame, Value, null);
                            break;
                        default:
                            throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
                    }
                }
                else
                {
                    // Otherwise, add to AdditonalHeaders[]
                    Frame.AdditionalHeaders.Add(new KeyValuePair<string, string>(Header, Value));
                }
                Read = Reader.SkipThrough('\r', '\n');

            } while (Read < 2 && !Reader.EOF);

            // Check for body + assign as needed
            if (Frame is StompBodiedFrame)
            {
                int BytePtr = Encoding.UTF8.GetByteCount(PacketAsString.Substring(0, Reader.Cursor));

                ((StompBodiedFrame)Frame)._PacketData = new byte[Packet.Length - BytePtr];
                Array.Copy(Packet, BytePtr, ((StompBodiedFrame)Frame)._PacketData, 0, Packet.Length - BytePtr);
            }

            return Frame;
        }
    }
}
