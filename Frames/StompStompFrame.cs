using System;

namespace STOMP.Frames
{
    [StompFrameType("STOMP", StompFrameDirection.ClientToServer)]
    public class StompStompFrame : StompConnectFrame
    {
        public StompStompFrame()
        {

        }
    }
}
