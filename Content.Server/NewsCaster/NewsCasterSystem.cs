using Content.Server.Chat;
using Content.Server.Listener;
using Content.Server.Popups;
using Content.Server.Station.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Speech;
using Robust.Shared.Player;

namespace Content.Server.NewsCaster
{
    public sealed class NewsCasterSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;
        [Dependency] private readonly StationSystem _stationSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<NewsCasterMicrophoneComponent, ComponentInit>(OnMicrophoneCompInit);
            SubscribeLocalEvent<NewsCasterSpeakerComponent, ComponentInit>(OnSpeakerCompInit);
            SubscribeLocalEvent<NewsCasterMicrophoneComponent, ActivateInWorldEvent>((uid, component, args) => ToggleNewsCasterMicrophone(uid, component, args.User));
            SubscribeLocalEvent<NewsCasterMicrophoneComponent, ExaminedEvent>(OnMicrophoneExamine);
            SubscribeLocalEvent<NewsCasterMicrophoneComponent, ListenerMessageHeardEvent>(OnMicrophoneHeardMessage);
        }

        private void OnMicrophoneCompInit(EntityUid uid, NewsCasterMicrophoneComponent comp, ComponentInit args)
        {
            EntityManager.EnsureComponent<ListenerComponent>(uid);
        }

        private void OnSpeakerCompInit(EntityUid uid, NewsCasterSpeakerComponent comp, ComponentInit args)
        {
            EntityManager.EnsureComponent<SharedSpeechComponent>(uid);
        }

        private void OnMicrophoneExamine(EntityUid uid, NewsCasterMicrophoneComponent comp, ExaminedEvent args)
        {
            if(!args.IsInDetailsRange) return;
            args.PushMarkup(
                comp.Enabled
                    ? Loc.GetString("newscaster-microphone-state-on")
                    : Loc.GetString("newscaster-microphone-state-off"));
        }

        private void ToggleNewsCasterMicrophone(EntityUid microphone, NewsCasterMicrophoneComponent comp, EntityUid user)
        {
            comp.Enabled = !comp.Enabled;
            _popupSystem.PopupEntity(
                comp.Enabled
                    ? Loc.GetString("newscaster-microphone-set-enabled")
                    : Loc.GetString("newscaster-microphone-set-disabled"),
                microphone, Filter.Entities(user));
        }

        private void OnMicrophoneHeardMessage(EntityUid uid, NewsCasterMicrophoneComponent comp, ListenerMessageHeardEvent args)
        {
            // We really don't want to repeat anything a newscaster speaker says
            if(EntityManager.HasComponent<NewsCasterSpeakerComponent>(args.Speaker)) return;

            var stationUid = _stationSystem.GetOwningStation(uid);
            foreach (var speakerComp in EntityManager.EntityQuery<NewsCasterSpeakerComponent>())
            {
                var speakerUid = speakerComp.Owner;
                if (_stationSystem.GetOwningStation(speakerUid) != stationUid) return;
                _chatSystem.TrySendInGameICMessage(speakerUid, args.Message, InGameICChatType.Speak, true);
            }
        }
    }
}
