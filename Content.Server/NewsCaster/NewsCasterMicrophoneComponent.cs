namespace Content.Server.NewsCaster
{
    [RegisterComponent]
    public sealed class NewsCasterMicrophoneComponent : Component
    {
        /// <summary>
        /// Is this microphone listening
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public bool Enabled = false;

        /// <summary>
        /// Distance at which voices will be picked up
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("listenRange")]
        public int ListenRange = 2;
    }
}
