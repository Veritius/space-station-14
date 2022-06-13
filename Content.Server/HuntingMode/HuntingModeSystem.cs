using Content.Shared.Damage.Prototypes;
using Content.Server.Weapon.Melee;
using Content.Shared.HuntingMode;
using Robust.Shared.Prototypes;

namespace Content.Server.HuntingMode
{
    public sealed class HuntingModeSystem : SharedHuntingModeSystem
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedHuntingModeComponent, MeleeHitEvent>(OnUnarmedHitEvent);
        }

        private void OnUnarmedHitEvent(EntityUid weapon, SharedHuntingModeComponent component, MeleeHitEvent args)
        {
            // TODO: This might need caching (as hit events can happen rapidly)
            var modifierPrototype = !component.IsInHuntingMode ? component.ActiveModifier : component.PassiveModifier;
            _prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierPrototype, out var modifier);
            if (modifier != null)
            {
                args.ModifiersList.Add(modifier);
            }
        }
    }
}
