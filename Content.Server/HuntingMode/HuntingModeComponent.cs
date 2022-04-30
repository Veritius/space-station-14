using Content.Shared.Damage.Prototypes;
using Content.Shared.HuntingMode;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.HuntingMode
{
    [RegisterComponent]
    public sealed class HuntingModeComponent : SharedHuntingModeComponent
    {
        [ViewVariables(VVAccess.ReadWrite)] [DataField("passiveModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
        public string PassiveModifier = "SerpentidPassiveStance";

        [ViewVariables(VVAccess.ReadWrite)] [DataField("activeModifier", customTypeSerializer:typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
        public string ActiveModifier = "SerpentidActiveStance";
    }
}
