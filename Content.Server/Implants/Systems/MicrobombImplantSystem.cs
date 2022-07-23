using Content.Shared.Actions;
using Content.Shared.Body.Components;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.Player;

namespace Content.Server.Implants;

public sealed class MicrobombImplantSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = null!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MicrobombImplantComponent, ComponentInit>(OnComponentAdded);
        SubscribeLocalEvent<MicrobombImplantComponent, ComponentShutdown>(OnComponentRemoved);
        SubscribeLocalEvent<MicrobombImplantComponent, MicrobombImplantComponent.MicrobombTriggeredActionEvent>(OnPerformAction);
    }

    public void OnComponentAdded(EntityUid uid, MicrobombImplantComponent comp, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, comp.Action, null);
    }

    public void OnComponentRemoved(EntityUid uid, MicrobombImplantComponent comp, ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(uid, comp.Action);
    }

    private void OnPerformAction(EntityUid uid, MicrobombImplantComponent component, MicrobombImplantComponent.MicrobombTriggeredActionEvent args)
    {
        _popupSystem.PopupCoordinates(Loc.GetString("microbomb-explode", ("person", uid)), Transform(uid).Coordinates, Filter.Pvs(uid), PopupType.Large);
        EntityManager.GetComponent<SharedBodyComponent>(uid).Gib();
    }
}
