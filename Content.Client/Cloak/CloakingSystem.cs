using Content.Shared.Cloak;
using Robust.Client.GameObjects;
using Robust.Shared.Reflection;

namespace Content.Client.Cloak
{
    public sealed class CloakingSystem : VisualizerSystem<CloakingVisualsComponent>
    {
        [Dependency] private readonly IReflectionManager _reflection = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<CloakingVisualsComponent, ComponentInit>(OnComponentAdded);
        }

        private void OnComponentAdded(EntityUid uid, CloakingVisualsComponent comp, ComponentInit args)
        {
            foreach (var rawLayer in comp._cloakLayersRaw)
            {
                if(!_reflection.TryParseEnumReference(rawLayer, out var layer)) continue;
                comp.CloakLayers.Add(layer);
            }
        }

        protected override void OnAppearanceChange(EntityUid uid, CloakingVisualsComponent component,
            ref AppearanceChangeEvent args)
        {
            if (args.Sprite == null)
                return;

            if (!args.Component.TryGetData(CloakingVisuals.IsCloaked, out bool isCloaked)) return;

            foreach (var enumObject in component.CloakLayers)
            {
                var layer = args.Sprite.LayerMapGet(enumObject);
                args.Sprite.LayerSetVisible(layer, isCloaked);
            }
        }
    }
}
