using Content.Server.CombatMode;
using Content.Server.GameTicking;
using Content.Server.Stunnable.Components;
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
        [Dependency] private readonly IEntityManager _entityManager = default!;

        public int ClownsBrutalisedCounter; // Goes up when a clown is hit
        // TODO: This may not make sense in future when/if disarms get reworked
        public int SecurityDisarmedCounter; // Goes up every time a security officer is disarmed
        public int SecurityHarmbatonCounter; // Goes up every time security uses harm baton

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<RoundRestartCleanupEvent>(Reset);
            SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEnd);
            SubscribeLocalEvent<MeleeHitEvent>(OnMeleeHit);
            SubscribeLocalEvent<RoundHighlightTrackerComponent, DisarmedEvent>(OnDisarm);
        }

        private void Reset(RoundRestartCleanupEvent args)
        {
            ClownsBrutalisedCounter = 0;
            ClownsBrutalisedCounter = 0;
            SecurityDisarmedCounter = 0;
            SecurityHarmbatonCounter = 0;
        }

        private void OnRoundEnd(RoundEndTextAppendEvent args)
        {
            if (!_configurationManager.GetCVar(CCVars.RoundHighlights)) return;

            // TODO: There's definitely a better way to do this
            if (ClownsBrutalisedCounter >= 1)
            {
                args.AddLine(Loc.GetString("round-highlight-clowns-brutalised", ("count", ClownsBrutalisedCounter)));
            }

            if (SecurityDisarmedCounter >= 1)
            {
                args.AddLine(Loc.GetString("round-highlight-security-disarms", ("count", SecurityDisarmedCounter)));
            }

            if (SecurityHarmbatonCounter >= 1)
            {
                args.AddLine(Loc.GetString("round-highlight-security-harmbaton", ("count", SecurityHarmbatonCounter)));
            }
        }

        private void OnMeleeHit(MeleeHitEvent args)
        {
            _entityManager.TryGetComponent<RoundHighlightTrackerComponent>(args.User, out var userComp);
            foreach (var entity in args.HitEntities)
            {
                _entityManager.TryGetComponent<RoundHighlightTrackerComponent>(entity, out var victimComp);

                // Track clown beatings
                if (victimComp.OwnerTags.Contains("clown"))
                {
                    ClownsBrutalisedCounter += 1;
                }

                // Track security harmbaton-ings
                if (userComp.OwnerTags.Contains("stunbaton"))
                {
                    _entityManager.TryGetComponent<StunbatonComponent>(userComp.Owner, out var batonComponent);
                    if (!batonComponent.Activated)
                    {
                        SecurityHarmbatonCounter += 1;
                    }
                }
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
