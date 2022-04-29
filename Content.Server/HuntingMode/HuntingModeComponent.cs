using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage.Prototypes;
using Content.Shared.HuntingMode;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.HuntingMode
{
    /// <summary>
    ///     Component given to serpentids that represents Hunting and Manipulation modes.
    /// </summary>
    [RegisterComponent]
    public sealed class HuntingModeComponent : SharedHuntingModeComponent
    {
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
