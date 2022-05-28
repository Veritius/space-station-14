using Robust.Shared.GameStates;

namespace Content.Client.Cloak
{
    [NetworkedComponent]
    [RegisterComponent]
    public class SharedCloakDistortionComponent : Component
    {
        private float _intensity;

        public float Intensity
        {
            get => _intensity;
            set
            {
                _intensity = value;
                Dirty();
            }
        }
    }
}
