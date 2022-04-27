using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;

namespace Content.Server.SerpentidAbilities
{
    /// <summary>
    ///     Component given to serpentids that represents Hunting and Manipulation modes.
    /// </summary>
    [RegisterComponent]
    public class HuntingModeComponent : Component
    {
        [ViewVariables]
        public bool IsInHuntingMode = false;

        [DataField("action")]
        public InstantAction ToggleAction = new()
        {
            Name = "action-name-hunting",
            Description = "action-description-hunting",
            UseDelay = TimeSpan.FromSeconds(2),
            CheckCanInteract = true,
            Event = new ToggleHuntingModeEvent()
        };
    }

    public sealed class ToggleHuntingModeEvent : InstantActionEvent { };
}
