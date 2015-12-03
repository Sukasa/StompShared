namespace STOMP.Frames
{
    /// <summary>
    ///     Ack frame for acknowledging the server.  Is handled automatically by the server for Messages
    /// </summary>
    [StompFrameType("ACK", StompFrameDirection.ClientToServer)]
    public class StompAckFrame : StompFrame
    {
        [StompHeaderIdentifier("id")]
        private string _Id;

        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        /// <summary>
        ///     Creates a new Ack frame for the given message Id
        /// </summary>
        public StompAckFrame()
        {

        }
    }
}
