namespace STOMP.Frames
{
    /// <summary>
    ///     STOMP Message frame that contains data sent from other nodes to this node via the server
    /// </summary>
    [StompFrameType("MESSAGE", StompFrameDirection.ServerToClient)]
    public class StompMessageFrame : StompBodiedFrame
    {
        [StompHeaderIdentifier("subscription")]
        internal string _Subscription = null;

        [StompHeaderIdentifier("destination")]
        internal string _Destination = null;

        [StompHeaderIdentifier("message-id")]
        internal string _MessageId = null;

        [StompHeaderIdentifier("ack")]
        internal string _Ack = null;

        /// <summary>
        ///     The Id of the subscription that this message was received as a part of
        /// </summary>
        public string SubscriptionId
        {
            get
            {
                return _Subscription;
            }
            set
            {
                _Subscription = value;
            }
        }

        /// <summary>
        ///     The destination address set from the sending client
        /// </summary>
        public string Destination
        {
            get
            {
                return _Destination;
            }
            set
            {
                _Destination = value;
            }
        }

        /// <summary>
        ///     A unique Id for the message, used for Ack/NAck packets
        /// </summary>
        public string MessageId
        {
            get
            {
                return _MessageId;
            }
            set
            {
                _MessageId = value;
            }
        }

        /// <summary>
        ///     What ACK string to send back to the server to acknowledge (or NAcknowledge) the packet
        /// </summary>
        public string Ack
        {
            get
            {
                return _Ack;
            }
            set
            {
                _Ack = value;
            }
        }

        public StompMessageFrame(string SubscriptionId, string Destination, int MessageId, string Ack = null)
        {
            _Subscription = SubscriptionId;
            _Destination = Destination;
            _MessageId = MessageId.ToString();
            _Ack = Ack;
        }
    }
}
