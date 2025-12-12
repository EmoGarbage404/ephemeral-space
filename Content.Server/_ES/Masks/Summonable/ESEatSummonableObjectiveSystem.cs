using Content.Server._ES.Masks.Summonable.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared._ES.Masks.Summonable.Components;
using Content.Shared._ES.Objectives;
using Content.Shared.Nutrition;
using Content.Shared.Objectives.Components;

namespace Content.Server._ES.Masks.Summonable;

/// <summary>
/// This handles <see cref="ESEatSummonableObjectiveComponent"/>
/// </summary>
public sealed class ESEatSummonableObjectiveSystem : ESBaseObjectiveSystem<ESEatSummonableObjectiveComponent>
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly ESSharedObjectiveSystem _objective = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESMaskSummonedComponent, FullyEatenEvent>(OnFullyEaten);
    }

    private void OnFullyEaten(Entity<ESMaskSummonedComponent> ent, ref FullyEatenEvent args)
    {
        if (Deleted(ent.Comp.OwnerMind))
            return;

        if (!_mind.TryGetMind(args.User, out var mind, out _) ||
            mind == ent.Comp.OwnerMind)
            return;

        foreach (var objective in _mind.ESGetObjectivesComp<ESEatSummonableObjectiveComponent>(ent.Comp.OwnerMind.Value))
        {
            _objective.AdjustObjectiveCounter(objective.Owner);
        }
    }
}
