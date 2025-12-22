using Content.Shared._ES.Objectives;
using Content.Shared._ES.Objectives.Components;
using Content.Shared._ES.Telesci.Components;
using Content.Shared.EntityTable;

namespace Content.Shared._ES.Telesci;

public abstract class ESSharedTelesciSystem : EntitySystem
{
    [Dependency] protected readonly EntityTableSystem EntityTable = default!;
    [Dependency] private readonly ESSharedObjectiveSystem _objective = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESTelesciObjectiveComponent, ESGetObjectiveProgressEvent>(OnGetObjectiveProgress);
    }

    private void OnGetObjectiveProgress(Entity<ESTelesciObjectiveComponent> ent, ref ESGetObjectiveProgressEvent args)
    {
        // Technically we CAN have multiple but this is unsupported behavior.
        foreach (var comp in EntityQuery<ESTelesciStationComponent>())
        {
            if (comp.MaxStage == 0)
                continue;
            args.Progress = (float) comp.Stage / comp.MaxStage;
            break;
        }
    }

    public void AdvanceTelesciStage(Entity<ESTelesciStationComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;
        SetTelesciStage(ent, ent.Comp.Stage + 1);
    }

    public void SetTelesciStage(Entity<ESTelesciStationComponent?> ent, int stageIdx)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        if (ent.Comp.Stage == stageIdx || stageIdx < 0 || stageIdx > ent.Comp.MaxStage)
            return;

        var stage = ent.Comp.Stages[stageIdx - 1];
        Log.Debug($"Advancing telesci stage: {stage.Danger}");
        // TODO: spawn events, etc.

        SpawnRewards((ent, ent.Comp), stage);

        ent.Comp.Stage = stageIdx;
        Dirty(ent);

        _objective.RefreshObjectiveProgress<ESTelesciObjectiveComponent>();
    }

    protected virtual void SpawnRewards(Entity<ESTelesciStationComponent> ent, ESTelesciStage stage)
    {

    }
}
