using System;

namespace STOMP.Frames
{
    /// <summary>
    ///     The same as a STOMP frame
    /// </summary>
    [StompFrameType("CONNECT", StompFrameDirection.ClientToServer)]
    public class StompConnectFrame : StompFrame
    {
        [StompHeaderIdentifier("accept-version")]
        internal string _AcceptedVersions = "1.2";

        [StompHeaderIdentifier("host")]
        internal string _Hostname;

        [StompHeaderIdentifier("login", true)]
        internal string _Username;

        [StompHeaderIdentifier("password", true)]
        internal string _Password;

        [StompHeaderIdentifier("heart-beat", true)]
        internal string _Heartbeat;

        public string Hostname
        {
            get { return _Hostname; }
            set { _Hostname = value; }
        }

        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        public string Heartbeat
        {
            get { return _Heartbeat; }
            set { _Heartbeat = value; }
        }

        public string Version
        {
            get { return _AcceptedVersions; }
            set { _AcceptedVersions = value; }
        }

        public  StompConnectFrame()
        {

        }
    }
}
