namespace Tptpd
{
    class PtpAnnounceMsg : PtpMsg
    {
        const int UTCOFFSET_OFFSET = 44;
        const int GM_P1_OFFSET = 47;
        const int GM_QUALITY_OFFSET = 48;
        const int GM_CLOCK_CLASS = 0;
        const int GM_CLOCK_ACCURACY = 1;
        const int GM_CLOCK_VARIANCE = 2;
        const int GM_P2_OFFSET = 52;
        const int GM_IDENTITY_OFFSET = 53;
        const int STEPS_OFFSET = 61;
        const int TIMESOURCE_OFFSET = 63;
        public const int ANNOUNCE_INTERVAL = 4000;
        public const int PORT = 320;
        Int16 currentUtcOffset;
        byte gmPriority1;
        byte[] gmClockQuality = new byte[4];
        byte gmPriority2;
        byte[] gmIdentity = new byte[8];
        Int16 stepsRemoved;
        byte timeSource;

        public PtpAnnounceMsg()
        {
            Random rnd = new Random();
            Length = 64;

            flags = new PtpFlags();
            flags.Set(PtpFlags.MsgFlags.TIME_TRACEABLE);
            flags.Set(PtpFlags.MsgFlags.PTP_TIMESCALE);
            flags.Set(PtpFlags.MsgFlags.PTP_UTC_REASONABLE);
            flags.Set(PtpFlags.MsgFlags.PTP_UNICAST);
            version = 2;
            messageType = (byte)ID.Announce;
            controlField = (byte)FIELD.Misc;
            logMessagePeriod = 1;
            sequenceId = (Int16)rnd.Next(0, Int16.MaxValue);
            Array.Fill<byte>(timestampS, 0);
            Array.Fill<byte>(timestampNs, 0);
            SourcePortID = 35;
            // NOTE this may be wrong
            currentUtcOffset = (Int16)(TimeZoneInfo.Local.BaseUtcOffset.Hours * 60);

            // TODO actually think about this value
            gmPriority1 = 1;
            gmClockQuality[GM_CLOCK_CLASS] = 6;
            gmClockQuality[GM_CLOCK_ACCURACY] = 0x30;
            Array.Fill<byte>(gmClockQuality, 0, GM_CLOCK_VARIANCE, 2);
            Array.Fill<byte>(gmIdentity, 0xFF);
            stepsRemoved = 0;
            timeSource = 0x60;
            gmPriority2 = 128;

        }

        public byte[] GetBytes()
        {
            byte[] data = GetBytesI();
            Array.Copy(ReverseArray(BitConverter.GetBytes(currentUtcOffset)), 0, data, UTCOFFSET_OFFSET, 2);
            data[GM_P1_OFFSET] = gmPriority1;
            Array.Copy(gmClockQuality, 0, data, GM_QUALITY_OFFSET, gmClockQuality.Length);
            data[GM_P2_OFFSET] = gmPriority2;
            Array.Copy(gmIdentity, 0, data, GM_IDENTITY_OFFSET, gmIdentity.Length);
            Array.Copy(ReverseArray(BitConverter.GetBytes(stepsRemoved)), 0, data, STEPS_OFFSET, 2);
            data[TIMESOURCE_OFFSET] = timeSource;
            return data;
        }

    }
}