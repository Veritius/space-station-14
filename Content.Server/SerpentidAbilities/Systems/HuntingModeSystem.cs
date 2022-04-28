using Content.Server.Objectives.Conditions;
using Content.Shared.Actions;
using Content.Server.Popups;
using Robust.Shared.Player;

namespace Content.Server.SerpentidAbilities
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
            _actionSystem.AddAction(uid, component.ToggleAction, null);
        }

        private void OnRemovedComponent(EntityUid uid, HuntingModeComponent component, ComponentShutdown args)
        {
            _actionSystem.RemoveAction(uid, component.ToggleAction);
        }

        private void OnPerformHuntingAction(EntityUid uid, HuntingModeComponent component, ToggleHuntingModeEvent args)
        {
            if (component.IsInHuntingMode)
            {
                component.IsInHuntingMode = false;
                _popupSystem.PopupEntity(Loc.GetString("hunting-to-manipulation-popup", ("person", uid)), uid, Filter.Pvs(uid));
            }
            else
            {
                component.IsInHuntingMode = true;
                _popupSystem.PopupEntity(Loc.GetString("manipulation-to-hunting-popup", ("person", uid)), uid, Filter.Pvs(uid));
            }
        }
    }
}
