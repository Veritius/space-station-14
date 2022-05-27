using Content.Shared.Actions;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Player;

namespace Content.Shared.HuntingMode
{
    public abstract class SharedHuntingModeSystem : EntitySystem
    {
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
        [Dependency] private readonly SharedActionsSystem _actionSystem = default!;
        [Dependency] private readonly SharedHandsSystem _sharedHands = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedHuntingModeComponent, PickupAttemptEvent>(TryPickupEvent);
            SubscribeLocalEvent<SharedHuntingModeComponent, ComponentInit>(OnGivenComponent);
            SubscribeLocalEvent<SharedHuntingModeComponent, ComponentShutdown>(OnRemovedComponent);
            SubscribeLocalEvent<SharedHuntingModeComponent, ToggleHuntingModeEvent>(OnPerformHuntingAction);
        }

        // TODO: Fix this not being predicted properly
        private void TryPickupEvent(EntityUid uid, SharedHuntingModeComponent component, PickupAttemptEvent args)
        {
            if (!component.IsInHuntingMode || !_tagSystem.HasTag(args.Item, "SerpentidHuntingUnusable")) return;
            _popupSystem.PopupEntity(Loc.GetString("hunting-cannot-pickup", ("item", args.Item)), uid, Filter.Entities(uid));
            args.Cancel();
        }

        private void OnGivenComponent(EntityUid uid, SharedHuntingModeComponent component, ComponentInit args)
        {
            _actionSystem.AddAction(uid, component.ToggleAction, null);
        }

        private void OnRemovedComponent(EntityUid uid, SharedHuntingModeComponent component, ComponentShutdown args)
        {
            _actionSystem.RemoveAction(uid, component.ToggleAction);
        }

        private void OnPerformHuntingAction(EntityUid uid, SharedHuntingModeComponent component, ToggleHuntingModeEvent args)
        {
            if (TryComp<SharedHandsComponent>(uid, out var handcomp))
            {
                foreach (var hand in handcomp.Hands)
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
