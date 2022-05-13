using Content.Server.CombatMode;
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
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;

        public int ClownsBrutalisedCounter; // Goes up when a clown is hit
        // TODO: This may not make sense in future when/if disarms get reworked
        public int SecurityDisarmedCounter; // Goes up every time a security officer is disarmed

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<RoundRestartCleanupEvent>(Reset);
            SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEnd);
            SubscribeLocalEvent<RoundHighlightTrackerComponent, MeleeHitEvent>(OnMeleeHit);
            SubscribeLocalEvent<RoundHighlightTrackerComponent, DisarmedEvent>(OnDisarm);
        }

        private void Reset(RoundRestartCleanupEvent args)
        {
            ClownsBrutalisedCounter = 0;
            SecurityDisarmedCounter = 0;
        }

        private void OnRoundEnd(RoundEndTextAppendEvent args)
        {
            if (!_configurationManager.GetCVar(CCVars.RoundHighlights)) return;
            args.AddLine(Loc.GetString("round-highlight-clowns-brutalised", ("count", ClownsBrutalisedCounter)));
            args.AddLine(Loc.GetString("round-highlight-security-disarms", ("count", SecurityDisarmedCounter)));
        }

        private void OnMeleeHit(EntityUid uid, RoundHighlightTrackerComponent component, MeleeHitEvent args)
        {
            if (component.OwnerTags.Contains("clown"))
            {
                ClownsBrutalisedCounter += 1;
            }
        }

        private void OnDisarm(EntityUid uid, RoundHighlightTrackerComponent component, DisarmedEvent args)
        {
            if (component.OwnerTags.Contains("security"))
            {
                SecurityDisarmedCounter += 1;
            }
        }
    }
}
