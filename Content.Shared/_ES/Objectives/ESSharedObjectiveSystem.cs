using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._ES.Objectives.Components;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._ES.Objectives;

/// <summary>
/// Handles assignment and core logic of objectives for ES.
/// </summary>
public abstract partial class ESSharedObjectiveSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly EntityTableSystem _entityTable = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        InitializeCounter();
    }

    /// <summary>
    /// Queries an objective to determine what its current progress is.
    /// </summary>
    public void RefreshObjectiveProgress(Entity<ESObjectiveComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        var ev = new ESGetObjectiveProgressEvent();
        RaiseLocalEvent(ent, ref ev);

        ent.Comp.Progress = Math.Clamp(ev.Progress, 0, 1);
        Dirty(ent);
    }

    /// <summary>
    /// Gets the current progress of an objective on [0, 1]
    /// </summary>
    public float GetProgress(Entity<ESObjectiveComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return 0;
        return ent.Comp.Progress;
    }

    /// <summary>
    /// Checks if a given objective is completed.
    /// </summary>
    public bool IsCompleted(Entity<ESObjectiveComponent?> ent)
    {
        return GetProgress(ent) >= 1;
    }

    /// <summary>
    /// Returns all objectives on an entity
    /// </summary>
    [PublicAPI]
    public List<Entity<ESObjectiveComponent>> GetObjectives(Entity<ESObjectiveHolderComponent?> ent)
    {
        return GetObjectives<ESObjectiveComponent>(ent);
    }

    /// <summary>
    /// Returns all objectives on an entity which have a given component
    /// </summary>
    public List<Entity<T>> GetObjectives<T>(Entity<ESObjectiveHolderComponent?> ent) where T : Component
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return [];

        var objectives = new List<Entity<T>>();
        foreach (var objective in ent.Comp.Objectives)
        {
            if (!TryComp<T>(objective, out var comp))
                continue;

            objectives.Add((objective, comp));
        }

        // TODO: eventify.

        return objectives;
    }

    /// <summary>
    /// <inheritdoc cref="CanAddObjective(Robust.Shared.GameObjects.Entity{Content.Shared._ES.Objectives.Components.ESObjectiveComponent?},Robust.Shared.GameObjects.Entity{Content.Shared._ES.Objectives.Components.ESObjectiveHolderComponent?})"/>
    /// </summary>
    [PublicAPI]
    public bool CanAddObjective(EntProtoId protoId, Entity<ESObjectiveHolderComponent?> holder)
    {
        var objectiveUid = EntityManager.PredictedSpawn(protoId, MapCoordinates.Nullspace);
        var objectiveComp = Comp<ESObjectiveComponent>(objectiveUid);
        var objectiveEnt = (objectiveUid, objectiveComp);

        var val = CanAddObjective(objectiveEnt, holder);

        // always destroy objectives created in this method.
        Del(objectiveUid);
        return val;
    }

    /// <summary>
    /// Checks if a given objective can be added
    /// </summary>
    public bool CanAddObjective(Entity<ESObjectiveComponent> ent, Entity<ESObjectiveHolderComponent?> holder)
    {
        // STUB: add events

        return true;
    }

    /// <summary>
    /// <inheritdoc cref="TryAddObjective(Robust.Shared.GameObjects.Entity{Content.Shared._ES.Objectives.Components.ESObjectiveHolderComponent?},Robust.Shared.Prototypes.EntProtoId,out Robust.Shared.GameObjects.Entity{Content.Shared._ES.Objectives.Components.ESObjectiveComponent}?)"/>
    /// </summary>
    public bool TryAddObjective(Entity<ESObjectiveHolderComponent?> ent, EntProtoId protoId)
    {
        return TryAddObjective(ent, protoId, out _);
    }

    /// <summary>
    /// Attempts to create and add multiple objectives
    /// </summary>
    /// <returns>Returns true if all objectives succeed</returns>
    public bool TryAddObjective(Entity<ESObjectiveHolderComponent?> ent, EntityTableSelector table)
    {
        var spawns = _entityTable.GetSpawns(table);
        return spawns.All(e => TryAddObjective(ent, e));
    }

    /// <summary>
    /// Attempts to create and assign an objective to an entity
    /// </summary>
    /// <param name="ent">The entity that will be assigned the objective</param>
    /// <param name="protoId">Prototype for the objective</param>
    /// <param name="objective">The newly created objective entity</param>
    public bool TryAddObjective(
        Entity<ESObjectiveHolderComponent?> ent,
        EntProtoId protoId,
        [NotNullWhen(true)] out Entity<ESObjectiveComponent>? objective)
    {
        objective = null;

        if (!Resolve(ent, ref ent.Comp))
            return false;

        var objectiveUid = EntityManager.PredictedSpawn(protoId, MapCoordinates.Nullspace);
        var objectiveComp = Comp<ESObjectiveComponent>(objectiveUid);
        objective = (objectiveUid, objectiveComp);

        if (!CanAddObjective(objective.Value, ent))
        {
            Del(objective);
            return false;
        }

        ent.Comp.Objectives.Add(objective.Value);
        RefreshObjectiveProgress(objective.Value.AsNullable());
        return true;
    }
}
