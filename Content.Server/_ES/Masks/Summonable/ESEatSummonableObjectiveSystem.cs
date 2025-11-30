using Content.Server._ES.Masks.Summonable.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared._ES.Masks.Summonable.Components;
using Content.Shared.Nutrition;
using Content.Shared.Objectives.Components;

namespace Content.Server._ES.Masks.Summonable;

/// <summary>
/// This handles <see cref="ESEatSummonableObjectiveComponent"/>
/// </summary>
public sealed class ESEatSummonableObjectiveSystem : EntitySystem
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESMaskSummonedComponent, FullyEatenEvent>(OnFullyEaten);
        SubscribeLocalEvent<ESEatSummonableObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
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
            objective.Comp.Eaten += 1;
        }
    }

    private void OnGetProgress(Entity<ESEatSummonableObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target == 0)
            return;
        args.Progress = Math.Clamp((float) ent.Comp.Eaten / target, 0, 1);
    }
}
