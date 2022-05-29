using Content.Server.DoAfter;
using Content.Shared.Actions;
using Robust.Server.GameObjects;

namespace Content.Server.Cloak
{
    public sealed class CloakingSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<CloakingComponent, ComponentInit>(OnComponentAdded);
            SubscribeLocalEvent<CloakingComponent, ComponentRemove>(OnComponentRemoved);
            SubscribeLocalEvent<CloakingComponent, ToggleCloakingEvent>(OnCloakingToggled);
            SubscribeLocalEvent<DoCloakEvent>(OnSuccessfullyCloaked);
            SubscribeLocalEvent<DoDecloakEvent>(OnSuccessfullyDecloaked);
        }

        private void OnComponentAdded(EntityUid uid, CloakingComponent comp, ComponentInit args)
        {
            _actionsSystem.AddAction(uid, comp.ToggleAction, null);
        }

        private void OnComponentRemoved(EntityUid uid, CloakingComponent comp, ComponentRemove args)
        {
            _actionsSystem.RemoveAction(uid, comp.ToggleAction);
        }

        private void OnCloakingToggled(EntityUid uid, CloakingComponent comp, ToggleCloakingEvent args)
        {
            object ev;
            float delay;

            if (comp.Cloaked)
            {
                ev = new DoDecloakEvent(comp);
                delay = comp.DecloakDelay;
            }
            else
            {
                ev = new DoCloakEvent(comp);
                delay = comp.CloakDelay;
            }

            if (delay <= 0f)
            {
                // Don't bother with doafter if there's no delay
                RaiseLocalEvent(ev);
            }
            else
            {
                var doAfterArgs = new DoAfterEventArgs(uid, delay)
                {
                    BreakOnDamage = false,
                    BreakOnStun = false,
                    BreakOnTargetMove = true,
                    BreakOnUserMove = true,
                    NeedHand = false,
                    BroadcastFinishedEvent = ev
                };

                _doAfterSystem.DoAfter(doAfterArgs);
            }
        }

        private void OnSuccessfullyCloaked(DoCloakEvent args)
        {
            ChangeCloakStatus(args.Comp, false);
        }

        private void OnSuccessfullyDecloaked(DoDecloakEvent args)
        {
            ChangeCloakStatus(args.Comp, false);
        }

        /// <summary>
        /// Sets whether or not the entity is cloaked.
        /// </summary>
        /// <param name="comp">The cloaking component</param>
        /// <param name="toState">Whether or not the entity should be cloaked</param>
        public void ChangeCloakStatus(CloakingComponent comp, bool toState)
        {
            var uid = comp.Owner;
            var visiblity = !toState;
            EntityManager.TryGetComponent<SpriteComponent>(uid, out var spriteComponent);
            for (int i = 0; i < spriteComponent.LayerCount; i++)
            {
                // TODO: Don't set ignored layers
                spriteComponent.LayerSetVisible(i, visiblity);
            }

            comp.Cloaked = toState;
        }
    }
}
