namespace Tptpd
{
    class PtpDelayRespMsg : PtpMsg
    {
        const int RECEIVE_S_OFFSET = LOGMESSAGE_OFFSET + 1;
        const int RECEIVE_NS_OFFSET = RECEIVE_S_OFFSET + 6;
        const int REQ_IDENTIFICATION_OFFSET = RECEIVE_NS_OFFSET + 4;
        const int REQ_SOURCEPORT_OFFSET = REQ_IDENTIFICATION_OFFSET + 8;
        public const int PORT = 320;
        byte[] SourcePortIdentity = new byte[8];
        Int16 SourcePortId;
        public PtpDelayRespMsg(byte[] req)
        {
            flags = new PtpFlags();
            Length = 54;
            messageType = (byte)ID.DelayResp;
            version = 2;
            flags.Set(PtpFlags.MsgFlags.PTP_UNICAST);
            SourcePortId = 35;
            sequenceId = req[SEQUENCEID_OFFSET];
            controlField = (byte)FIELD.DelayResp;

            Array.Copy(req, CLOCKIDENTITY_OFFSET, SourcePortIdentity, 0, SourcePortIdentity.Length);
            SourcePortID = req[SOURCEPORT_OFFSET];
            logMessagePeriod = req[LOGMESSAGE_OFFSET];

        }

        public byte[] GetBytes()
        {
            SetTimeStamp();
            byte[] data = GetBytesI();
            Array.Copy(SourcePortIdentity, 0, data, REQ_IDENTIFICATION_OFFSET, SourcePortIdentity.Length);
            Array.Copy(ReverseArray(BitConverter.GetBytes(SourcePortID)), 0, data, REQ_SOURCEPORT_OFFSET, 2);
            return data;
        }

    }
}