using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Server.Cloak
{
    /// <summary>
    /// Makes people invisible through altering opacity
    /// </summary>
    /// <remarks>
    /// TODO: Replace this with a cool shader like in https://cdn.discordapp.com/attachments/770682801607278632/980005437120331816/shitty_stelth.mp4 when engine is less ass
    /// </remarks>
    [RegisterComponent]
    public sealed class CloakingComponent : Component, ISerializationHooks
    {
        /// <summary>
        /// Defines whether the entity is cloaked at the moment
        /// </summary>
        /// <remarks>
        /// DO NOT SET THIS YOURSELF. Use CloakingSystem instead.
        /// </remarks>
        [ViewVariables]
        [DataField("startCloaked")]
        public bool Cloaked;

        /// <summary>
        /// How long it takes to turn on cloak (doafter)
        /// Leave at 0 to skip the doafter
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("cloakDelay")]
        public float CloakDelay;

        /// <summary>
        /// How long it takes to turn off cloak (doafter)
        /// Leave at 0 to skip the doafter
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("decloakDelay")]
        public float DecloakDelay;

        /// <summary>
        /// Layers to affect
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public List<Enum> IgnoreLayers = new();

        [DataField("alwaysVisibleLayers", true)]
        private List<string> _alwaysVisibleLayers = default!;

        /// <summary>
        /// The InstantAction that toggles cloak
        /// </summary>
        [DataField("action")]
        public InstantAction ToggleAction = new()
        {
            Name = "action-name-toggle-cloak",
            Description = "action-description-toggle-cloak",
            // TODO: This is a placeholder sprite, replace it
            Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/Alerts/Weightless/weightless.png")),
            UseDelay = TimeSpan.FromSeconds(20),
            CheckCanInteract = false,
            Event = new ToggleCloakingEvent()
        };

        // TODO: surely there's a better way to do this that doesn't use obsolete methods
        void ISerializationHooks.AfterDeserialization()
        {
            var reflectionManager = IoCManager.Resolve<IReflectionManager>();
            foreach (var rawLayer in _alwaysVisibleLayers)
            {
                if (reflectionManager.TryParseEnumReference(rawLayer, out var layers))
                    IgnoreLayers.Add(layers);
            }
        }
    }

    public sealed class ToggleCloakingEvent : InstantActionEvent { };
}
