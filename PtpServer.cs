using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tptpd
{

    // TODO implement multicast
    class PtpServer
    {
        const int PTP_DEFAULT_LISTEN_PORT = 319;
        const int PTP_DEFAULT_SEND_PORT = 320;
        IPEndPoint endpoint;
        List<Timer> broadcastTimer;
        List<IPAddress> unicastClients;
        object sequenceLock;
        Int16 sequenceId;
        Thread listenerThread;
        bool running;

        public PtpServer()
        {
            unicastClients = new List<IPAddress>();
            broadcastTimer = new List<Timer>();
            sequenceLock = new object();
            endpoint = new IPEndPoint(IPAddress.Any, 319);
        }

        void ListenerThread()
        {
            Logger.Trace("ListenerThread enter");
            using UdpClient listener = new UdpClient(PTP_DEFAULT_LISTEN_PORT);
            while (running)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 319);
                byte[] delayReqData = listener.Receive(ref sender);
                PtpDelayRespMsg respMsg = new PtpDelayRespMsg(delayReqData);
                byte[] delayResp = respMsg.GetBytes();
                listener.Connect(sender.Address, PTP_DEFAULT_SEND_PORT);
                listener.Send(delayResp, delayResp.Length);
            }
            Logger.Trace("ListenerThread exit");
        }

        public bool AddClient(string clientIP)
        {
            Logger.Trace("AddClient enter");
            IPAddress ip = IPAddress.Parse(clientIP);
            if (unicastClients.Contains(ip))
                return false;

            unicastClients.Add(ip);
            Logger.Trace("AddClient exit");
            return true;
        }

        // TODO Add PtpAnnounce
        public void Start()
        {
            Logger.Trace("Start enter");
            listenerThread = new Thread(new ThreadStart(ListenerThread));
            listenerThread.Start();
            foreach (var client in unicastClients)
            {
                Timer timer = new Timer(SyncFollowUpRoutine, client, 0, 1000);
                broadcastTimer.Add(timer);
            }
            Console.ReadLine();
            running = false;
            Logger.Trace("Start exit");
        }

        void SyncFollowUpRoutine(Object _clientIP)
        {
            IPAddress clientIP = (IPAddress)_clientIP;
            Logger.Trace($"SyncFollowUpRoutine enter {clientIP.ToString()}");
            PtpSyncMsg syncMsg;
            PtpFollowUpMsg followUpMsg;
            using UdpClient syncClient = new UdpClient();
            using UdpClient followupClient = new UdpClient();

            lock (sequenceLock)
            {
                syncMsg = new PtpSyncMsg(sequenceId);
                followUpMsg = new PtpFollowUpMsg(sequenceId);
                sequenceId++;
            }

            syncClient.Connect(clientIP, PtpSyncMsg.PORT);
            followupClient.Connect(clientIP, PtpFollowUpMsg.PORT);
            byte[] syncBytes = syncMsg.GetBytes();
            syncClient.Send(syncBytes, syncBytes.Length);
            followUpMsg.SetTimeStamp();
            byte[] followUpBytes = followUpMsg.GetBytes();
            followupClient.Send(followUpBytes, followUpBytes.Length);
            Logger.Trace("SyncFollowUpRoutine exit");
        }

#if DEBUG
        void CreateTestPackets()
        {
            Logger.Trace("CreateTestPackets enter");
            byte[] reqData = new byte[44];
            Array.Fill<byte>(reqData, 0xFF);

            using UdpClient syncClient = new UdpClient();
            using UdpClient followupClient = new UdpClient();
            using UdpClient delayRespClient = new UdpClient();
            syncClient.Connect(IPAddress.Loopback, PtpSyncMsg.PORT);
            followupClient.Connect(IPAddress.Loopback, PtpFollowUpMsg.PORT);
            delayRespClient.Connect(IPAddress.Loopback, PtpDelayRespMsg.PORT);

            PtpSyncMsg syncMsg = new PtpSyncMsg(sequenceId);
            PtpFollowUpMsg followupMsg = new PtpFollowUpMsg(sequenceId);
            PtpDelayRespMsg delayRespMsg = new PtpDelayRespMsg(reqData);

            syncClient.Send(syncMsg.GetBytes(), syncMsg.Length);
            followupClient.Send(followupMsg.GetBytes(), followupMsg.Length);
            delayRespClient.Send(delayRespMsg.GetBytes(), delayRespMsg.Length);
            Logger.Trace("CreateTestPackets exit");
        }
#endif
    }
}