using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.SerpentidAbilities
{
    /// <summary>
    ///     Component given to serpentids that represents Hunting and Manipulation modes.
    /// </summary>
    [RegisterComponent]
    public class HuntingModeComponent : Component
    {
        [ViewVariables]
        public bool IsInHuntingMode = false;

        [ViewVariables(VVAccess.ReadWrite)] [DataField("passiveModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
        public string PassiveModifier = "SerpentidPassiveStance";

        [ViewVariables(VVAccess.ReadWrite)] [DataField("activeModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
        public string ActiveModifier = "SerpentidActiveStance";

        public InstantAction ToggleAction = new()
        {
            Name = "action-name-hunting",
            Description = "action-description-hunting",
            UseDelay = TimeSpan.FromSeconds(2),
            CheckCanInteract = true,
            Event = new ToggleHuntingModeEvent()
        };
    }

    public sealed class ToggleHuntingModeEvent : InstantActionEvent { };
}
