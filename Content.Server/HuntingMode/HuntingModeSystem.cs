using Content.Server.Hands.Components;
using Content.Shared.Actions;
using Content.Shared.Damage.Prototypes;
using Content.Server.Popups;
using Content.Server.Weapon.Melee;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.HuntingMode;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.HuntingMode
{
    /// <summary>
    ///     Takes care of hunting and manipulation mode for serpentids.
    /// </summary>
    public sealed class HuntingModeSystem : SharedHuntingModeSystem
    {
        [Dependency] private readonly SharedActionsSystem _actionSystem = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedHandsSystem _sharedHands = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<HuntingModeComponent, ComponentInit>(OnGivenComponent);
            SubscribeLocalEvent<HuntingModeComponent, ComponentShutdown>(OnRemovedComponent);
            SubscribeLocalEvent<HuntingModeComponent, ToggleHuntingModeEvent>(OnPerformHuntingAction);
            SubscribeLocalEvent<HuntingModeComponent, MeleeHitEvent>(OnUnarmedHitEvent); }

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
            if (TryComp<HandsComponent>(uid, out var handcomp))
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

        private void OnUnarmedHitEvent(EntityUid weapon, HuntingModeComponent component, MeleeHitEvent args)
        {
            // TODO: This might need caching (as hit events can happen rapidly)
            var modifierprototype = !component.IsInHuntingMode ? component.ActiveModifier : component.PassiveModifier;
            _prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierprototype, out var modifier);
            if (modifier != null)
            {
                args.ModifiersList.Add(modifier);
            }
        }
    }
}
