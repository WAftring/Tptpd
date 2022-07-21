namespace Tptpd
{
    class PtpFollowUpMsg : PtpMsg
    {
        public const int PORT = 320;
        const int MSG_SIZE = 44;
        public PtpFollowUpMsg(Int16 _sequenceId)
        {
            messageType = (byte)ID.FollowUp;
            version = 2;
            Length = MSG_SIZE;
            flags = new PtpFlags();
            flags.Set(PtpFlags.MsgFlags.PTP_UNICAST);
            SourcePortID = 35;
            sequenceId = _sequenceId;
            controlField = (byte)FIELD.FollowUp;
        }
        public byte[] GetBytes() { return GetBytesI(); }
    }
}