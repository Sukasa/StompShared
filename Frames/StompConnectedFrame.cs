namespace STOMP.Frames
{
    [StompFrameType("CONNECTED", StompFrameDirection.ServerToClient)]
    public class StompConnectedFrame : StompFrame
    {
        [StompHeaderIdentifier("heartbeat", true)]
        internal string _Heartbeat = "0,0";

        [StompHeaderIdentifier("session", true)]
        internal string _SessionId = null;

        [StompHeaderIdentifier("server", true)]
        internal string _ServerInfo = "Unknown/1.0";

        [StompHeaderIdentifier("version")]
        internal string _Version = "1.2";

        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = value;
            }
        }

        public string Heartbeat
        {
            get
            {
                return _Heartbeat;
            }
            set
            {
                _Heartbeat = value;
            }
        }

        public string Session
        {
            get
            {
                return _SessionId;
            }
            set
            {
                _SessionId = value;
            }
        }

        public string ServerInfo
        {
            get
            {
                return _ServerInfo;
            }
            set
            {
                _ServerInfo = value;
            }
        }

        public StompConnectedFrame() { }
    }
}
