﻿using System.Linq;
using Content.Server.Cargo.Systems;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Players;
using Content.Server.RandomAppearance;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Spawners.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.CCVar;
using Content.Shared.CharacterAppearance;
using Content.Shared.Roles;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// This handles the Pirates minor antag, which is designed to coincide with other modes on occasion.
/// </summary>
public sealed class PiratesRuleSystem : GameRuleSystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IMapLoader _mapLoader = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawningSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly PricingSystem _pricingSystem = default!;

    [ViewVariables]
    private List<Mind.Mind> _pirates = new();
    [ViewVariables]
    private EntityUid _pirateShip = EntityUid.Invalid;
    [ViewVariables]
    private HashSet<EntityUid> _initialItems = new();
    [ViewVariables]
    private double _initialShipValue;

    public override string Prototype => "Pirates";

    private const string PiratePrototypeId = "Pirate";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RulePlayerSpawningEvent>(OnPlayerSpawningEvent);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEndTextEvent);
    }

    private void OnRoundEndTextEvent(RoundEndTextAppendEvent ev)
    {
        if (!Enabled)
            return;

        if (Deleted(_pirateShip))
        {
            // Major loss, the ship somehow got annihilated.
            ev.AddLine(Loc.GetString("pirates-no-ship"));
        }
        else
        {

            List<(double, EntityUid)> mostValuableThefts = new();

            var finalValue = _pricingSystem.AppraiseGrid(_pirateShip, uid =>
            {
                foreach (var mind in _pirates)
                {
                    if (mind.CurrentEntity == uid)
                        return false; // Don't appraise the pirates twice, we count them in separately.
                }
                return true;
            }, (uid, price) =>
            {
                if (_initialItems.Contains(uid))
                    return;

                mostValuableThefts.Add((price, uid));
                mostValuableThefts.Sort((i1, i2) => i2.Item1.CompareTo(i1.Item1));
                if (mostValuableThefts.Count > 5)
                    mostValuableThefts.Pop();
            });

            foreach (var mind in _pirates)
            {
                if (mind.CurrentEntity is not null)
                    finalValue += _pricingSystem.GetPrice(mind.CurrentEntity.Value);
            }

            var score = finalValue - _initialShipValue;

            ev.AddLine(Loc.GetString("pirates-final-score", ("score", $"{score:F2}")));
            ev.AddLine(Loc.GetString("pirates-final-score-2", ("finalPrice", $"{finalValue:F2}")));

            ev.AddLine("");
            ev.AddLine(Loc.GetString("pirates-most-valuable"));

            foreach (var (price, obj) in mostValuableThefts)
            {
                ev.AddLine(Loc.GetString("pirates-stolen-item-entry", ("entity", obj), ("credits", $"{price:F2}")));
            }

            if (mostValuableThefts.Count == 0)
                ev.AddLine(Loc.GetString("pirates-stole-nothing"));
        }

        ev.AddLine("");
        ev.AddLine(Loc.GetString("pirates-list-start"));
        foreach (var pirates in _pirates)
        {
            ev.AddLine($"- {pirates.CharacterName} ({pirates.Session?.Name})");
        }
    }

    public override void Started(GameRuleConfiguration _) { }

    public override void Ended(GameRuleConfiguration _) { }

    private void OnPlayerSpawningEvent(RulePlayerSpawningEvent ev)
    {
        if (!Enabled)
            return;

        _pirates.Clear();
        _initialItems.Clear();

        // Basically copied verbatim from nukie code
        var playersPerPirate = _cfg.GetCVar(CCVars.PiratesPlayersPerOp);
        var maxPirates = _cfg.GetCVar(CCVars.PiratesMaxOps);

        var everyone = new List<IPlayerSession>(ev.PlayerPool);
        var prefList = new List<IPlayerSession>();
        var pirates = new List<IPlayerSession>();

        // The LINQ expression ReSharper keeps suggesting is completely unintelligible so I'm disabling it
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var player in everyone)
        {
            if(player.ContentData()?.Mind?.AllRoles.All(role => role is not Job {CanBeAntag: false}) ?? false) continue;
            if (!ev.Profiles.ContainsKey(player.UserId))
            {
                continue;
            }
            var profile = ev.Profiles[player.UserId];
            if (profile.AntagPreferences.Contains(PiratePrototypeId))
            {
                prefList.Add(player);
            }
        }

        var numPirates = MathHelper.Clamp(ev.PlayerPool.Count / playersPerPirate, 1, maxPirates);

        for (var i = 0; i < numPirates; i++)
        {
            if (prefList.Count == 0)
            {
                if (everyone.Count == 0)
                {
                    Logger.InfoS("preset", "Insufficient ready players to fill up with pirates, stopping the selection");
                    break;
                }
                pirates.Add(_random.PickAndTake(everyone));
                Logger.InfoS("preset", "Insufficient preferred nukeops, picking at random.");
            }
            pirates.Add(_random.PickAndTake(prefList));
        }

        // TODO: Make this a prototype
        var map = "/Maps/pirate.yml";

        var aabbs = _stationSystem.Stations.SelectMany(x =>
            Comp<StationDataComponent>(x).Grids.Select(x => _mapManager.GetGridComp(x).Grid.WorldAABB)).ToArray();
        var aabb = aabbs[0];

        for (var i = 1; i < aabbs.Length; i++)
        {
            aabb.Union(aabbs[i]);
        }

        var (_, gridId) = _mapLoader.LoadBlueprint(GameTicker.DefaultMap, map, new MapLoadOptions
        {
            Offset = aabb.Center + MathF.Max(aabb.Height / 2f, aabb.Width / 2f) * 2.5f
        });

        if (!gridId.HasValue)
        {
            Logger.ErrorS("pirates", $"Gridid was null when loading \"{map}\", aborting.");
            foreach (var session in pirates)
            {
                ev.PlayerPool.Add(session);
            }
            return;
        }

        _pirateShip = _mapManager.GetGridEuid(gridId.Value);

        // TODO: Loot table or something
        var pirateGear = _prototypeManager.Index<StartingGearPrototype>("PirateGear"); // YARRR

        var spawns = new List<EntityCoordinates>();

        // Forgive me for hardcoding prototypes
        foreach (var (_, meta, xform) in EntityQuery<SpawnPointComponent, MetaDataComponent, TransformComponent>(true))
        {
            if (meta.EntityPrototype?.ID != "SpawnPointPirates" || xform.ParentUid != _pirateShip) continue;

            spawns.Add(xform.Coordinates);
        }

        if (spawns.Count == 0)
        {
            spawns.Add(Transform(_pirateShip).Coordinates);
            Logger.WarningS("pirates", $"Fell back to default spawn for pirates!");
        }

        for (var i = 0; i < pirates.Count; i++)
        {
            var sex = _random.Prob(0.5f) ? Sex.Male : Sex.Female;

            var name = sex.GetName("Human", _prototypeManager, _random);

            var session = pirates[i];
            var newMind = new Mind.Mind(session.UserId)
            {
                CharacterName = name
            };
            newMind.ChangeOwningPlayer(session.UserId);

            var mob = Spawn("MobHuman", _random.Pick(spawns));
            MetaData(mob).EntityName = name;
            EntityManager.AddComponent<RandomAppearanceComponent>(mob);

            newMind.TransferTo(mob);
            _stationSpawningSystem.EquipStartingGear(mob, pirateGear, null);

            _pirates.Add(newMind);

            GameTicker.PlayerJoinGame(session);
        }

        _initialShipValue = _pricingSystem.AppraiseGrid(_pirateShip, uid =>
        {
            _initialItems.Add(uid);
            return true;
        }); // Include the players in the appraisal.
    }

    private void OnStartAttempt(RoundStartAttemptEvent ev)
    {
        if (!Enabled)
            return;

        var minPlayers = _cfg.GetCVar(CCVars.PiratesMinPlayers);
        if (!ev.Forced && ev.Players.Length < minPlayers)
        {
            _chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-not-enough-ready-players", ("readyPlayersCount", ev.Players.Length), ("minimumPlayers", minPlayers)));
            ev.Cancel();
            return;
        }

        if (ev.Players.Length == 0)
        {
            _chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-no-one-ready"));
            ev.Cancel();
            return;
        }
    }
}
