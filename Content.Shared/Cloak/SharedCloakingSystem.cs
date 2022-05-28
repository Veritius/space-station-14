using Content.Client.Cloak;

namespace Content.Shared.Cloak
{
    public class SharedCloakingSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedCloakDistortionComponent, ComponentInit>(OnDistortionComponentAdded);
            SubscribeLocalEvent<SharedCloakDistortionComponent, ComponentRemove>(OnDistortionComponentRemoved);

        }

        private void OnDistortionComponentAdded(EntityUid uid, SharedCloakDistortionComponent component, ComponentInit args)
        {

        }

        private void OnDistortionComponentRemoved(EntityUid uid, SharedCloakDistortionComponent component, ComponentRemove args)
        {

        }
    }
}

