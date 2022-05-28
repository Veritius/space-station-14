using Content.Client.Interactable.Components;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client.Cloak
{
    public sealed class CloakDistortionSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ClientCloakDistortionComponent, ComponentInit>(OnComponentAdded);
            SubscribeLocalEvent<ClientCloakDistortionComponent, ComponentRemove>(OnComponentRemoved);
        }

        private void OnComponentAdded(EntityUid uid, ClientCloakDistortionComponent comp, ComponentInit args)
        {
            if (EntityManager.TryGetComponent<InteractionOutlineComponent>(uid, out var outlineComp))
            {
                comp.OwnerHadInteractionOutlineComp = true;
                EntityManager.RemoveComponent<InteractionOutlineComponent>(uid);
            }
            else
            {
                comp.OwnerHadInteractionOutlineComp = false;
            }

            var instance = _prototypeManager.Index<ShaderPrototype>("Cloaked").Instance().Duplicate();
            comp.Shader = instance;
        }

        private void OnComponentRemoved(EntityUid uid, ClientCloakDistortionComponent comp, ComponentRemove args)
        {
            if (comp.OwnerHadInteractionOutlineComp)
            {
                EntityManager.AddComponent<InteractionOutlineComponent>(uid);
            }
        }
    }
}

