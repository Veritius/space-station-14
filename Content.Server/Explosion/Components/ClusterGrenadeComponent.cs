using Content.Server.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
    [RegisterComponent, Friend(typeof(ClusterGrenadeSystem))]
    public sealed class ClusterGrenadeComponent : Component
    {
        public Container GrenadesContainer = default!;

        /// <summary>
        ///     What we fill our prototype with if we want to pre-spawn with grenades.
        /// </summary>
        [ViewVariables] [DataField("fillPrototype", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? FillPrototype;

        /// <summary>
        ///     If we have a pre-fill how many more can we spawn.
        /// </summary>
        public int UnspawnedCount;

        /// <summary>
        ///     Maximum grenades in the container.
        /// </summary>
        [ViewVariables] [DataField("maxGrenadesCount")]
        public int MaxGrenades = 3;

        /// <summary>
        ///     How long until our grenades are shot out and armed.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)] [DataField("delay")]
        public float Delay = 1;

        /// <summary>
        ///     Max distance grenades can be thrown.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)] [DataField("distance")]
        public float ThrowDistance = 50;

        /// <summary>
        /// How many grenades are thrown at once.
        /// Set to 0 to throw all at once.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)] [DataField("throwsPerSecond")]
        public int ThrowsPerSecond = 0;

        /// <summary>
        ///     This is the end.
        /// </summary>
        public bool CountDown;
    }
}
