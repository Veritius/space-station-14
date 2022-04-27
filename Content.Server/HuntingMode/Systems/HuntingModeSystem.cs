using Content.Shared.Actions;
using Content.Server.Popups;

namespace Content.Server.HuntingMode
{
    /// <summary>
    ///     Takes care of hunting and manipulation mode for serpentids.
    /// </summary>
    public sealed class HuntingModeSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actionSystem = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<HuntingModeComponent, ComponentInit>(OnGivenComponent);
            SubscribeLocalEvent<HuntingModeComponent, ComponentShutdown>(OnRemovedComponent);
            SubscribeLocalEvent<HuntingModeComponent, ToggleHuntingModeEvent>(OnPerformHuntingAction);
        }

        private void OnGivenComponent(EntityUid uid, HuntingModeComponent component, ComponentInit args)
        {
            _actionSystem.AddAction(uid, component.Action, null);
        }

        private void OnRemovedComponent(EntityUid uid, HuntingModeComponent component, ComponentShutdown args)
        {
            _actionSystem.RemoveAction(uid, component.Action);
        }

        private void OnPerformHuntingAction(EntityUid uid, HuntingModeComponent component, ToggleHuntingModeEvent args)
        {
            if (component.IsInHuntingMode)
            {
                component.IsInHuntingMode = false;
                _popupSystem.PopupEntity(Loc.GetString("hunting-to-manipulation-popup"), uid, Filter.Entities(uid));
            }
            else
            {
                component.IsInHuntingMode = true;
                _popupSystem.PopupEntity(Loc.GetString("manipulation-to-hunting-popup"), uid, Filter.Entities(uid));
            }
        }
    }
}
