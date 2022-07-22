using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace Tptpd
{

    class PtpFlags
    {
        [Flags]
        public enum MsgFlags
        {
            PTP_LI_61 = 0x0001,
            PTP_LI_59 = 0x0002,
            PTP_UTC_REASONABLE = 0x0004,
            PTP_TIMESCALE = 0x0008,
            TIME_TRACEABLE = 0x0010,
            FREQUENCY_TRACEABLE = 0x0020,
            SYNCHRONIZATION_UNCERTAIN = 0x0040,
            PTP_ALTERNATE_MASTER = 0x0100,
            PTP_TWO_STEP = 0x0200,
            PTP_UNICAST = 0x0400,
            PTP_PROFILE1 = 0x2000,
            PTP_PROILE2 = 0x4000,
            PTP_SECURITY = 0x8000
        }
        public MsgFlags value { get; private set; }
        public void Set(MsgFlags flag) { value |= flag; }
        public void Unset(MsgFlags flag) { value &= ~flag; }
    }
    class PtpMsg
    {
        internal const int MESSAGETYPE_OFFSET = 0x0;
        internal const int VERSION_OFFSET = 0x1;
        internal const int Length_OFFSET = 0x2;
        internal const int DOMAIN_OFFSET = 0x4;
        //const int MINORSDO_OFFSET = 0x6;
        internal const int FLAGS_OFFSET = 0x6;
        internal const int CORRECTIONFIELD_OFFSET = 0x8;
        //const int MESSAGETYPESPECIFIC_OFFSET = 0x11;
        internal const int CLOCKIDENTITY_OFFSET = 0x14;
        internal const int SOURCEPORT_OFFSET = 0x1C;
        internal const int SEQUENCEID_OFFSET = 0x1E;
        internal const int CONTROLFIELD_OFFSET = 0x20;
        internal const int LOGMESSAGE_OFFSET = 0x21;
        internal const int TIMESTAMP_S_OFFSET = 0x22;
        internal const int TIMESTAMP_NS_OFFSET = 0x28;
        internal enum ID
        {
            Sync = 0x0,
            DelayReq = 0x1,
            FollowUp = 0x8,
            DelayResp = 0x9,
            Announce = 0xB

        }
        internal enum FIELD
        {
            Sync = 0,
            DelayReq = 1,
            FollowUp = 2,
            DelayResp = 3,
            Management = 4,
            Misc = 5
        }
        internal byte messageType;
        internal byte version;
        public Int16 Length;
        internal byte domainNumber;
        internal byte minorSdoId;
        internal PtpFlags flags;
        internal byte[] correctionField = new byte[6];
        internal Int32 messageTypeSpecific;
        internal Int64 ClockIdentity;
        internal Int16 SourcePortID;
        internal Int16 sequenceId;
        internal byte controlField;
        internal byte logMessagePeriod;
        internal byte[] timestampS = new byte[6];
        internal byte[] timestampNs = new byte[4];
        internal int MsgSize;
        internal virtual byte[] GetBytesI()
        {
            byte[] data = new byte[Length];

            data[MESSAGETYPE_OFFSET] = messageType;
            data[VERSION_OFFSET] = version;
            Array.Copy(ReverseArray(BitConverter.GetBytes(Length)), 0, data, Length_OFFSET, 2);
            Array.Copy(ReverseArray(BitConverter.GetBytes((Int16)flags.value)), 0, data, FLAGS_OFFSET, 2);
            // TODO figure out the right way to deal with this
            Array.Fill<byte>(data, 0x57, CLOCKIDENTITY_OFFSET, 8);
            //Array.Copy(ReverseArray(BitConverter.GetBytes(ClockIdentity)), 0, data, CLOCKIDENTITY_OFFSET, 8);
            Array.Copy(ReverseArray(BitConverter.GetBytes(SourcePortID)), 0, data, SOURCEPORT_OFFSET, 2);
            Array.Copy(ReverseArray(BitConverter.GetBytes(sequenceId)), 0, data, SEQUENCEID_OFFSET, 2);
            data[CONTROLFIELD_OFFSET] = controlField;
            data[LOGMESSAGE_OFFSET] = logMessagePeriod;
            Array.Copy(timestampS, 0, data, TIMESTAMP_S_OFFSET, timestampS.Length);
            Array.Copy(timestampNs, 0, data, TIMESTAMP_NS_OFFSET, timestampNs.Length);

            return data;
        }
        internal byte[] ReverseArray(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        public virtual void SetTimeStamp()
        {
            // ASSUMPTION(will): We are going to use the constructor as the time that the PTP Sync packet was sent
            TimeSpan ts = DateTime.UtcNow - DateTime.UnixEpoch;
            TimeSpan tns = ts.Subtract(TimeSpan.FromSeconds(ts.Seconds));

            // Artificially trimming the size
            UInt32 epochSec = (UInt32)ts.TotalSeconds;
            UInt32 epochNs = (UInt32)tns.Ticks * 100;
            byte[] tempSec = BitConverter.GetBytes(epochSec);
            byte[] tempNs = BitConverter.GetBytes(epochNs);
#if DEBUG
            Console.WriteLine($"Epoch Sec: {epochSec}\nEpoch NS: {epochNs}");
#endif
            Array.Copy(ReverseArray(tempSec), 0, timestampS, 2, tempSec.Length);
            Array.Copy(ReverseArray(tempNs), timestampNs, tempNs.Length);
        }
    }
}