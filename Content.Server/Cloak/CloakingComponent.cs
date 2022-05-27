using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Cloak;
using Robust.Shared.Utility;

namespace Content.Server.Cloak
{
    public class CloakingComponent : SharedCloakingComponent
    {
        /// <summary>
        /// If the entity's owner had the interation overlay component
        /// Used for bypassing the renderer limitations
        /// </summary>
        public bool OwnerHadInteractionOverlayComp;

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
