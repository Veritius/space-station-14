using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Player;

namespace Content.Shared.HuntingMode
{
    public abstract class SharedHuntingModeSystem : EntitySystem
    {
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<SharedHuntingModeComponent, PickupAttemptEvent>(TryPickupEvent);
        }

        private void TryPickupEvent(EntityUid uid, SharedHuntingModeComponent component, PickupAttemptEvent args)
        {
            if (!component.IsInHuntingMode || _tagSystem.HasTag(args.Item, "SerpentidHuntingUsable")) return;
            _popupSystem.PopupEntity(Loc.GetString("hunting-cannot-pickup", ("item", args.Item)), uid, Filter.Entities(uid));
            args.Cancel();
        }
    }
}
