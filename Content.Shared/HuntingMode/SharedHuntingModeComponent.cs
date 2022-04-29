using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;

namespace Content.Shared.HuntingMode
{
    public class SharedHuntingModeComponent : Component
    {
        [ViewVariables]
        public bool IsInHuntingMode = false;

        public InstantAction ToggleAction = new()
        {
            Name = "action-name-hunting",
            Description = "action-description-hunting",
            UseDelay = TimeSpan.FromSeconds(2),
            CheckCanInteract = true,
            Event = new ToggleHuntingModeEvent()
        };
    }

    public sealed class ToggleHuntingModeEvent : InstantActionEvent {}
}
