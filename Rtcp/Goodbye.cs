﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Media.Rtcp
{
    public sealed class Goodbye : RtcpPacket
    {
        #region Properties

        public uint SynchronizationSourceIdentifier { get { return Utility.ReverseUnsignedInt(BitConverter.ToUInt32(Payload, 0)); } set { BitConverter.GetBytes(Utility.ReverseUnsignedInt(value)).CopyTo(Payload, 0); } }

        public string Reason
        {
            get { return Encoding.UTF8.GetString(Payload, 5, Payload[4]); }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Payload = BitConverter.GetBytes(Utility.ReverseUnsignedInt(SynchronizationSourceIdentifier));
                }
                else
                {
                    if (value.Length > 255) throw new ArgumentException("Reason cannot be longer than 255 characters.");
                    byte[] values = Encoding.UTF8.GetBytes(value);
                    if (value.Length == Reason.Length)
                    {
                        values.CopyTo(Payload, 5);
                    }
                    else
                    {
                        List<byte> temp = new List<byte>();
                        temp.AddRange(Payload, 0, 4);
                        temp.Add((byte)values.Length);
                        temp.AddRange(Encoding.UTF8.GetBytes(value));
                        Payload = temp.ToArray();
                    }
                }
            }
        }

        #endregion

        #region Constructor

        public Goodbye(uint ssrc, byte? channel = null) : base(RtcpPacketType.Goodbye, channel) { SynchronizationSourceIdentifier = ssrc; }

        public Goodbye(RtcpPacket packet) : base(packet) { if (packet.PacketType != RtcpPacket.RtcpPacketType.Goodbye) throw new Exception("Invalid Packet Type, Expected Goodbye. Found: '" + (byte)packet.PacketType + '\''); }

        public Goodbye(byte[] packet, int index) : base(packet, index, RtcpPacketType.Goodbye) { }

        #endregion
    }
}
