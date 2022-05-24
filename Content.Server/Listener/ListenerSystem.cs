using Content.Server.Interaction;

namespace Content.Server.Listener
{
    public class ListenerSystem : EntitySystem
    {
        [Dependency] private readonly InteractionSystem _interactionSystem = default!;

        /// <summary>
        /// Raises ListenerMessageHeardEvent on all ListenerComponents within their ranges
        /// </summary>
        /// <param name="source">The entity that sent the message</param>
        /// <param name="message">The message contents</param>
        public void CheckAllListeners(EntityUid source, string message)
        {
            foreach (var comp in EntityManager.EntityQuery<ListenerComponent>())
            {
                if (!_interactionSystem.InRangeUnobstructed(comp.Owner, source, comp.ListenDistance)) return;
                var ev = new ListenerMessageHeardEvent(source, message);
                RaiseLocalEvent(comp.Owner, ev, false);
            }
        }
    }
}
