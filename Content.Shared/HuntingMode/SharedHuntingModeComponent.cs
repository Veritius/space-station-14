using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.HuntingMode
{
    [RegisterComponent]
    public sealed class SharedHuntingModeComponent : Component
    {
        [ViewVariables]
        public bool IsInHuntingMode = false;

        [ViewVariables] [DataField("passiveModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
        public string PassiveModifier = "SerpentidPassiveStance";

        [ViewVariables] [DataField("activeModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
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

    public sealed class ToggleHuntingModeEvent : InstantActionEvent {}
}
