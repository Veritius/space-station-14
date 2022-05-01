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
    public sealed class HuntingModeSystem : SharedHuntingModeSystem
    {
        [Dependency] private readonly SharedActionsSystem _actionSystem = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedHandsSystem _sharedHands = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedHuntingModeComponent, MeleeHitEvent>(OnUnarmedHitEvent);
        }

        private void OnUnarmedHitEvent(EntityUid weapon, SharedHuntingModeComponent component, MeleeHitEvent args)
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
