using Content.Server.Body.Components;
using Content.Server.DoAfter;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Cloak;
using Content.Shared.Database;
using Robust.Server.GameObjects;

namespace Content.Server.Cloak
{
    public sealed class CloakingSystem : SharedCloakingSystem
    {
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
        [Dependency] private readonly SharedAdminLogSystem _adminLog = default!;

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
            _adminLog.Add(LogType.Action, $"{EntityManager.ToPrettyString(args.Comp.Owner):entity} has cloaked themselves");
            ChangeCloakStatus(args.Comp, false);
        }

        private void OnSuccessfullyDecloaked(DoDecloakEvent args)
        {
            _adminLog.Add(LogType.Action, $"{EntityManager.ToPrettyString(args.Comp.Owner):entity} has decloaked themselves");
            ChangeCloakStatus(args.Comp, false);
        }

        /// <summary>
        /// Sets whether or not the entity is cloaked.
        /// </summary>
        /// <param name="comp">The cloaking component</param>
        /// <param name="toState">Whether or not the entity should be cloaked</param>
        public void ChangeCloakStatus(CloakingComponent comp, bool toState)
        {
            if (toState)
            {
                EntityUid uid = comp.Owner;
                comp.Cloaked = true;
            }
            else
            {
                EntityUid uid = comp.Owner;
                comp.Cloaked = false;
            }
        }
    }
}
