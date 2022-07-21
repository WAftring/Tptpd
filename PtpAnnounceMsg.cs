namespace Tptpd
{
    class PtpAnnounceMsg : PtpMsg
    {
        const int UTCOFFSET_OFFSET = 44;
        const int GM_P1_OFFSET = 47;
        const int GM_QUALITY_OFFSET = 48;
        const int GM_P2_OFFSET = 52;
        const int GM_IDENTITY_OFFSET = 53;
        const int STEPS_OFFSET = 61;
        const int TIMESOURCE_OFFSET = 63

        Int16 currentUtcOffset;
        byte gmPriority1;
        Int32 gmClockQuality;
        byte gmPriority2;
        byte[] gmIdentity = new byte[8];
        Int16 stepsRemoved;
        byte timeSource;

        // TODO (will): Implement me
        public PtpAnnounceMsg()
        {

        }

    }
}