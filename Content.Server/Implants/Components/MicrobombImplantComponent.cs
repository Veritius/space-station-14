using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Utility;

namespace Content.Server.Implants;

[RegisterComponent]
public sealed class MicrobombImplantComponent : Component
{
    public InstantAction Action = new()
    {
        Name = "action-name-microbomb",
        Description = "action-description-microbomb",
        Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/Alerts/Fire/fire.png")),
        CheckCanInteract = false,
        Event = new MicrobombTriggeredActionEvent(),
    };

    public sealed class MicrobombTriggeredActionEvent : InstantActionEvent {}
}
