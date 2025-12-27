using Content.Shared._ES.Objectives;
using Content.Shared._ES.Objectives.Components;
using Content.Shared._ES.Telesci.Components;
using Content.Shared.EntityTable;
using Content.Shared.Gravity;
using Content.Shared.Station;

namespace Content.Shared._ES.Telesci;

public abstract class ESSharedTelesciSystem : EntitySystem
{
    [Dependency] protected readonly EntityTableSystem EntityTable = default!;
    [Dependency] private readonly SharedGravitySystem _gravity = default!;
    [Dependency] private readonly ESSharedObjectiveSystem _objective = default!;
    [Dependency] private readonly SharedStationSystem _station = default!;

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

        SpawnEvents((ent, ent.Comp), stage);
        SpawnRewards((ent, ent.Comp), stage);
        SendAnnouncement(ent, stage);

        // TODO: replace with real screen shake once we have it
        foreach (var grid in _station.GetGrids(ent.Owner))
        {
            _gravity.StartGridShake(grid);
        }

        ent.Comp.Stage = stageIdx;
        Dirty(ent);

        TryCallShuttle((ent, ent.Comp));

        _objective.RefreshObjectiveProgress<ESTelesciObjectiveComponent>();
    }

    protected virtual void SpawnEvents(Entity<ESTelesciStationComponent> ent, ESTelesciStage stage)
    {

    }

    protected virtual void SpawnRewards(Entity<ESTelesciStationComponent> ent, ESTelesciStage stage)
    {

    }

    protected virtual void SendAnnouncement(EntityUid ent, ESTelesciStage stage)
    {

    }

    protected virtual bool TryCallShuttle(Entity<ESTelesciStationComponent> ent)
    {
        return ent.Comp.Stage >= ent.Comp.MaxStage;
    }
}
