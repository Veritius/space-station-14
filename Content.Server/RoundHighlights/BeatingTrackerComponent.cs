namespace Content.Server.RoundHighlight
{
    /// <summary>
    /// A tag-component for RoundSummarySystem.
    /// </summary>
    [RegisterComponent]
    public sealed class BeatingTrackerComponent : Component
    {
        /// <summary>
        /// What stats should be altered when the owner is beaten.
        /// </summary>
        [DataField("tags")]
        public List<string> OwnerTags = new List<string>();
    }
}
