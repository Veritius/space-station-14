using Content.Shared.Actions;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Player;

namespace Content.Shared.HuntingMode
{
    public abstract class HuntingModeSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
        [Dependency] private readonly SharedActionsSystem _actionSystem = default!;
        [Dependency] private readonly SharedHandsSystem _sharedHands = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<HuntingModeComponent, PickupAttemptEvent>(TryPickupEvent);
            SubscribeLocalEvent<HuntingModeComponent, ComponentInit>(OnGivenComponent);
            SubscribeLocalEvent<HuntingModeComponent, ComponentShutdown>(OnRemovedComponent);
            SubscribeLocalEvent<HuntingModeComponent, ToggleHuntingModeEvent>(OnPerformHuntingAction);
        }

        // TODO: Fix this not being predicted properly
        private void TryPickupEvent(EntityUid uid, HuntingModeComponent component, PickupAttemptEvent args)
        {
            if (!component.IsInHuntingMode || HasComp<UnusableWhileHuntingComponent>(args.Item)) return;
            _popupSystem.PopupEntity(Loc.GetString("hunting-cannot-pickup", ("item", args.Item)), uid, Filter.Entities(uid));
            args.Cancel();
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
            if (TryComp<SharedHandsComponent>(uid, out var handComp))
            {
                foreach (var hand in handComp.Hands)
                {
                    _sharedHands.TrySetActiveHand(uid, hand.Key);
                    _sharedHands.TryDrop(uid);
                }
            }
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
