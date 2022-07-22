namespace Tptpd
{
    [Serializable]
    class PtpSyncMsg : PtpMsg
    {
        public const int PORT = 319;
        const int MSG_SIZE = 44;
        public PtpSyncMsg(Int16 _sequenceId)
        {
            flags = new PtpFlags();
            messageType = (byte)ID.Sync;
            version = 2;
            Length = MSG_SIZE;
            flags.Set(PtpFlags.MsgFlags.PTP_UNICAST);
            SourcePortID = 35;
            sequenceId = _sequenceId;
            controlField = (byte)FIELD.Sync;
        }

        public byte[] GetBytes() { return GetBytesI(); }
    }
}