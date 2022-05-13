using Content.Server.GameTicking;
using Content.Server.Weapon.Melee;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;

namespace Content.Server.RoundHighlight
{
    /// <summary>
    /// Keeps track of specific events throughout the round and presents highlights at the end.
    /// </summary>
    public sealed class RoundSummarySystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;

        public int ClownsBrutalisedCounter; // Goes up when a clown is hit

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<RoundRestartCleanupEvent>(Reset);
            SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEnd);
            SubscribeLocalEvent<RoundHighlightTrackerComponent, MeleeHitEvent>(OnMeleeHit);
        }

        private void Reset(RoundRestartCleanupEvent args)
        {
            ClownsBrutalisedCounter = 0;
        }

        private void OnRoundEnd(RoundEndTextAppendEvent args)
        {
            if (_configurationManager.GetCVar(CCVars.RoundHighlights))
            {
                args.AddLine(Loc.GetString("round-summary-clowns-brutalised", ("count", ClownsBrutalisedCounter)));
            }
        }

        private void OnMeleeHit(EntityUid uid, RoundHighlightTrackerComponent component, MeleeHitEvent args)
        {
            if (component.OwnerTags.Contains("clown"))
            {
                ClownsBrutalisedCounter += 1;
            }
        }
    }
}
