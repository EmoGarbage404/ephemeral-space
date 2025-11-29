using Content.Shared._ES.Masks.Summonable.Components;
using Content.Shared.Actions;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Examine;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._ES.Masks.Summonable;

public sealed class ESContainerSummonableSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedEntityStorageSystem _entityStorage = default!;
    [Dependency] private readonly EntityTableSystem _entityTable = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESMaskSummonedComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<ESContainerSummonActionEvent>(OnContainerSummonAction);
    }

    private void OnExamined(Entity<ESMaskSummonedComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.ExamineString is not { } str)
            return;

        if (!_mind.TryGetMind(args.Examiner, out var mind, out _) ||
            mind != ent.Comp.OwnerMind)
            return;

        args.PushMarkup(Loc.GetString(str));
    }

    private void OnContainerSummonAction(ESContainerSummonActionEvent args)
    {
        if (!TryComp<EntityStorageComponent>(args.Target, out var storage) ||
            _entityStorage.IsOpen(args.Target, storage))
            return;

        _mind.TryGetMind(args.Performer, out var mind, out _);

        var spawns = _entityTable.GetSpawns(args.Table);
        foreach (var spawn in spawns)
        {
            var ent = PredictedSpawnInContainerOrDrop(spawn, args.Target, storage.Contents.ID);
            var comp = EnsureComp<ESMaskSummonedComponent>(ent);
            comp.OwnerMind = mind;
            comp.ExamineString = args.ExamineString;
        }

        _popup.PopupClient(Loc.GetString("es-container-summonable-summon-popup", ("target", args.Target)), args.Target, args.Performer);
        _audio.PlayLocal(args.SummonSound, args.Target, args.Performer);

        args.Handled = true;
    }
}

public sealed partial class ESContainerSummonActionEvent : EntityTargetActionEvent
{
    [DataField]
    public EntityTableSelector Table = new NoneSelector();

    [DataField]
    public SoundSpecifier? SummonSound = new SoundPathSpecifier("/Audio/Items/toolbox_drop.ogg");

    [DataField]
    public LocId? ExamineString;
}
