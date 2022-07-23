using Robust.Shared.Prototypes;

namespace Content.Server.Implants;

// ReSharper disable all RedundantLinebreak
[RegisterComponent]
public sealed class AddComponentsToTargetComponent : Component
{
    public bool Injecting;

    /// <summary>
    /// How long of a doafter should the item have. Set to zero for no doafter.
    /// </summary>
    [DataField("delay")] [ViewVariables(VVAccess.ReadOnly)]
    public float Delay;

    /// <summary>
    /// Components to add to the target.
    /// </summary>
    [DataField("components")] [ViewVariables(VVAccess.ReadOnly)]
    public EntityPrototype.ComponentRegistry Components { get; } = new();

    /// <summary>
    /// How many charges are left in the device. -1 skips the check.
    /// </summary>
    [DataField("charges")] [ViewVariables(VVAccess.ReadWrite)]
    public int RemainingCharges = 1;
}
