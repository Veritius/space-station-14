namespace Content.Server.Listener
{
    /// <summary>
    /// Raises events for chat messages that fulfill basic requirements
    /// </summary>
    [RegisterComponent]
    public class ListenerComponent : Component
    {
        /// <summary>
        /// Maximum distance at which messages will be heard
        /// </summary>
        [DataField("distance")]
        public int ListenDistance = 7;
    }

    public class ListenerMessageHeardEvent : EntityEventArgs
    {
        /// <summary>
        /// The entity that sent the message
        /// </summary>
        public EntityUid Speaker;
        /// <summary>
        /// The message contents
        /// </summary>
        public string Message;

        public ListenerMessageHeardEvent(EntityUid speaker, string message)
        {
            Speaker = speaker;
            Message = message;
        }
    }
}
