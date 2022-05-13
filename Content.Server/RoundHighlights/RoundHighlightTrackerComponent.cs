namespace Content.Server.RoundHighlight
{
    /// <summary>
    /// A tag-component for RoundHighlightSystem.
    /// </summary>
    [RegisterComponent]
    public sealed class RoundHighlightTrackerComponent : Component
    {
        /// <summary>
        /// Defines what stats should be tracked
        /// </summary>
        [DataField("tags")]
        public List<string> OwnerTags = new();
    }
}
