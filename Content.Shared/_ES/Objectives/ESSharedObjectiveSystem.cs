using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Objectives.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
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
    [Dependency] private readonly SharedPvsOverrideSystem _pvsOverride = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESObjectiveComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<ESObjectiveComponent> ent, ref MapInitEvent args)
    {
        InitializeTargetValue(ent);
    }

    private void InitializeTargetValue(Entity<ESObjectiveComponent> ent)
    {
        // No variation in target, no further logic needed
        if (ent.Comp.MaxTarget is not { } maxTarget)
            return;

        // Generate a random value on [target, maxTarget] in chunks of targetIncrement
        var range = maxTarget - ent.Comp.Target;
        var incrementCount = (int) Math.Ceiling((range / ent.Comp.TargetIncrement).Float());
        var blend = _random.Next(0, incrementCount + 1); // non-inclusive right bound adjustment
        ent.Comp.Target = FixedPoint2.Clamp(ent.Comp.Target + blend * ent.Comp.TargetIncrement, ent.Comp.Target, maxTarget);
        Dirty(ent);
    }

    /// <summary>
    /// Attempts to create and initialize an objective.
    /// </summary>
    /// <param name="protoId">Prototype for the objective</param>
    /// <param name="objective">Objective entity out param</param>
    /// <param name="holder">Optional argument for an objective holder. Can be used in assignment to exclude entities as targets</param>
    public bool TryCreateObjective(
        EntProtoId<ESObjectiveComponent> protoId,
        [NotNullWhen(true)] out Entity<ESObjectiveComponent>? objective,
        Entity<ESObjectiveHolderComponent?>? holder = null)
    {
        var objectiveUid = EntityManager.PredictedSpawn(protoId, MapCoordinates.Nullspace);
        var objectiveComp = Comp<ESObjectiveComponent>(objectiveUid);
        objective = (objectiveUid, objectiveComp);

        if (false)
        {
            Del(objective);
            return false;
        }

        return true;
    }

    public bool TryAssignObjective(
        Entity<ESObjectiveHolderComponent?> ent,
        Entity<ESObjectiveComponent>? objective)
    {
        return false;
    }

    /// <summary>
    /// Attempts to create and assign an objective to an entity
    /// </summary>
    /// <param name="ent">The entity that will be assigned the objective</param>
    /// <param name="protoId">Prototype for the objective</param>
    /// <param name="objective">The newly created objective entity</param>
    public bool TryCreateAndAssignObjective(
        Entity<ESObjectiveHolderComponent?> ent,
        EntProtoId<ESObjectiveComponent> protoId,
        [NotNullWhen(true)] out Entity<ESObjectiveComponent>? objective)
    {
        objective = null;

        if (Resolve(ent, ref ent.Comp))
            return false;

        if (!TryCreateObjective(protoId, out objective, holder: ent))
            return false;

        if (!TryAssignObjective(ent, objective))
        {
            Del(objective);
            return false;
        }

        return true;
    }
}
