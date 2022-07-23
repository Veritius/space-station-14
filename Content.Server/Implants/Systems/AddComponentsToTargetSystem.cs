using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.Player;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Implants;

public sealed class AddComponentsToTargetSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = null!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = null!;
    [Dependency] private readonly IComponentFactory _componentFactory = null!;
    [Dependency] private readonly ISerializationManager _serializationManager = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AddComponentsToTargetComponent, UseInHandEvent>(OnUseInHandEvent);
        SubscribeLocalEvent<AddComponentsToTargetComponent, AfterInteractEvent>(OnAfterInteractEvent);
        SubscribeLocalEvent<AddComponentsToTargetSuccessEvent>(OnAddComponentDoafterSuccess);
        SubscribeLocalEvent<AddComponentsToTargetCancelledEvent>(OnAddComponentDoafterFailure);
        SubscribeLocalEvent<AddComponentsToTargetComponent, ExaminedEvent>(OnExaminedEvent);
    }

    private void OnUseInHandEvent(EntityUid uid, AddComponentsToTargetComponent comp, UseInHandEvent args)
    {
        if (args.Handled) return;
        args.Handled = true;
        AddComponents(args.User, args.User, comp);
    }

    private void OnAfterInteractEvent(EntityUid uid, AddComponentsToTargetComponent comp, AfterInteractEvent args)
    {
        if (args.Handled || args.Target == null || !args.CanReach) return;
        args.Handled = true;
        switch (comp.RemainingCharges)
        {
            case 0:
                _popupSystem.PopupEntity(Loc.GetString("implanter-remaining-charges-plain", ("charges", "no")), args.User, Filter.Entities(args.User));
                break;
            default:
                AddComponents(args.User, args.Target.Value, comp);
                break;
        }
    }

    private void OnAddComponentDoafterSuccess(AddComponentsToTargetSuccessEvent args)
    {
        if(args.Component.RemainingCharges != -1) { args.Component.RemainingCharges -= 1; }
        foreach (var (name, data) in args.Component.Components)
        {
            var component = (Component) _componentFactory.GetComponent(name);
            component.Owner = args.Target;

            var copied = (Component?) _serializationManager.Copy(data.Component, component, null);
            if (copied != null)
                EntityManager.AddComponent(args.Target, copied);
        }
    }

    private void OnAddComponentDoafterFailure(AddComponentsToTargetCancelledEvent args)
    {
        args.Component.Injecting = false;
    }

    private void AddComponents(EntityUid user, EntityUid target, AddComponentsToTargetComponent comp)
    {
        if (comp.Injecting) return;
        comp.Injecting = true;

        _doAfterSystem.DoAfter(new DoAfterEventArgs(user, comp.Delay, target: target)
        {
            BroadcastFinishedEvent = new AddComponentsToTargetSuccessEvent(user, target, comp),
            BroadcastCancelledEvent = new AddComponentsToTargetCancelledEvent(target, comp),
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
        });
    }

    private void OnExaminedEvent(EntityUid uid, AddComponentsToTargetComponent comp, ExaminedEvent args)
    {
        switch (comp.RemainingCharges)
        {
            case 0:
                args.PushMarkup(Loc.GetString("implanter-remaining-charges", ("color", "red"), ("charges", "no")));
                break;
            case -1:
                args.PushMarkup(Loc.GetString("implanter-remaining-charges", ("color", "cyan"), ("charges", "unlimited")));
                break;
            default:
                args.PushMarkup(Loc.GetString("implanter-remaining-charges", ("color", "green"), ("charges", comp.RemainingCharges)));
                break;
        }
    }

    private sealed class AddComponentsToTargetSuccessEvent : EntityEventArgs
    {
        public EntityUid User { get; }
        public EntityUid Target { get; }
        public AddComponentsToTargetComponent Component { get; }

        public AddComponentsToTargetSuccessEvent(EntityUid user, EntityUid target, AddComponentsToTargetComponent component)
        {
            User = user;
            Target = target;
            Component = component;
        }
    }

    private sealed class AddComponentsToTargetCancelledEvent : EntityEventArgs
    {
        public EntityUid Target { get; }
        public AddComponentsToTargetComponent Component { get; }

        public AddComponentsToTargetCancelledEvent(EntityUid target, AddComponentsToTargetComponent component)
        {
            Target = target;
            Component = component;
        }
    }
}
