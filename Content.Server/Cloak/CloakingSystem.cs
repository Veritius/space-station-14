using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Cloak;

namespace Content.Server.Cloak
{
    public sealed class CloakingSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

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
            UpdateAppearance(uid, false, comp);
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
                ev = new DoDecloakEvent(uid, comp);
                delay = comp.DecloakDelay;
            }
            else
            {
                ev = new DoCloakEvent(uid, comp);
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
            if(!_entityManager.TryGetComponent<AppearanceComponent>(args.Uid, out var appearanceComp)) return;
            UpdateAppearance(args.Uid, true, args.Comp, appearanceComp);
        }

        private void OnSuccessfullyDecloaked(DoDecloakEvent args)
        {
            if(!_entityManager.TryGetComponent<AppearanceComponent>(args.Uid, out var appearanceComp)) return;
            UpdateAppearance(args.Uid, false, args.Comp, appearanceComp);
        }

        private void UpdateAppearance(EntityUid uid,
            bool cloaked,
            CloakingComponent? cloakComp = null,
            AppearanceComponent? appearance = null)
        {
            if (!Resolve(uid, ref cloakComp, ref appearance, false))
                return;

            appearance.SetData(CloakingVisuals.IsCloaked, cloaked);
        }
    }
}
