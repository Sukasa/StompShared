namespace STOMP.Frames
{
    /// <summary>
    ///     The Subscription frame type for subscribing to a feed on the server
    /// </summary>
    [StompFrameType("SUBSCRIBE", StompFrameDirection.ClientToServer)]
    public class StompSubscribeFrame : StompFrame
    {
        [StompHeaderIdentifier("destination")]
        internal string _Destination;

        [StompHeaderIdentifier("id")]
        internal string _Id;

        [StompHeaderIdentifier("ack", true)]
        internal string _Ack = "client-individual";

        /// <summary>
        ///     What kind of acknowledgement setting the client wants associated with the subscription
        /// </summary>
        public string AckSetting
        {
            get
            {
                return _Ack;
            }
        }

        /// <summary>
        ///     Which feed to subscribe to
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
        ///     A unique, client-generated Id for this particular subscription
        /// </summary>
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
        ///     Creates a new Subscription frame with the specified Destination and Id
        /// </summary>
        /// <param name="Destination">
        ///     Which feed to subscribe to
        /// </param>
        /// <param name="Id">
        ///     A unique, client-generated Id for this particular subscription
        /// </param>
        public StompSubscribeFrame(string Destination, string Id)
        {
            _Destination = Destination;
            _Id = Id;
        }
    }
}
