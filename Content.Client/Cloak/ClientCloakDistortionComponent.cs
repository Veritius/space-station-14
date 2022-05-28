using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client.Cloak
{
    public sealed class ClientCloakDistortionComponent : SharedCloakDistortionComponent
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public bool OwnerHadInteractionOutlineComp;
        public ShaderInstance? Shader;
    }
}

