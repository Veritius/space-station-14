using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Utility;

namespace Content.Server.Cloak
{
    [RegisterComponent]
    public sealed class CloakingComponent : Component
    {
        /// <summary>
        /// If the entity's owner had the interaction overlay component
        /// Used for bypassing the renderer limitations
        /// </summary>
        public bool OwnerHadInteractionOverlayComp;

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
    }

    public sealed class ToggleCloakingEvent : InstantActionEvent { };
}
