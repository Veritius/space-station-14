namespace Content.Client.Cloak
{
    [RegisterComponent]
    public sealed class CloakingVisualsComponent : Component
    {
        /// <summary>
        /// Layers to affect
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public List<Enum> CloakLayers = new();

        [DataField("cloakLayers", true)]
        public List<string> _cloakLayersRaw = default!;
    }
}
