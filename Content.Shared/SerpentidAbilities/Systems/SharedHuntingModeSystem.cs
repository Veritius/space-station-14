using Content.Shared.Hands.Components;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Player;

namespace Content.Shared.SerpentidAbilities
{
    public class SharedHuntingModeSystem : EntitySystem
    {
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedHandsComponent, PickupAttemptEvent>(TryPickupEvent);
        }

        private void TryPickupEvent(EntityUid uid, SharedHandsComponent component, PickupAttemptEvent args)
        {
            EntityManager.TryGetComponent(args.User, out SharedHuntingModeComponent huntcomp);
            if (huntcomp.IsInHuntingMode && !_tagSystem.HasTag(args.Item, "SerpentidHuntingUsable"))
            {
                _popupSystem.PopupEntity(Loc.GetString("hunting-cannot-pickup", ("item", args.Item)), uid, Filter.Entities(uid));
                args.Cancel();
            }
        }
    }
}
