
namespace STOMP.Frames
{
    [StompFrameType("NACK", StompFrameDirection.ClientToServer)]
    public class StompNAckFrame : StompAckFrame
    {
        public StompNAckFrame()
        {

        }

        public StompNAckFrame(StompMessageFrame Frame)
        {
            Id = Frame.Ack;
        }

    }
}
