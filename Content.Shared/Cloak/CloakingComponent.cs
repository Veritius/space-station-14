namespace Content.Shared.Cloak
{
    /// <summary>
    /// Invisibility!
    /// </summary>
    public sealed class CloakingComponent : Component
    {
        /// <summary>
        /// Is the entity currently cloaked
        /// </summary>
        public bool Cloaked;

        /// <summary>
        /// How long it takes to turn on cloak (doafter)
        /// Leave at 0 to skip the doafter
        /// </summary>
        [DataField("cloakDelay")]
        public float CloakDelay;

        /// <summary>
        /// How long it takes to turn off cloak (doafter)
        /// Leave at 0 to skip the doafter
        /// </summary>
        [DataField("decloakDelay")]
        public float DecloakDelay;
    }
}
