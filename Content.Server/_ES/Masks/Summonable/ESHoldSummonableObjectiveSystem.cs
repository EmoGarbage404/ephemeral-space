using Content.Server._ES.Masks.Summonable.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared._ES.Masks.Summonable.Components;
using Content.Shared.Objectives.Components;
using Robust.Server.Containers;

namespace Content.Server._ES.Masks.Summonable;

/// <summary>
/// This handles <see cref="ESHoldSummonableObjectiveComponent"/>
/// </summary>
public sealed class ESHoldSummonableObjectiveSystem : EntitySystem
{
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESHoldSummonableObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnGetProgress(Entity<ESHoldSummonableObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target == 0)
            return;

        var held = 0;

        var query = EntityQueryEnumerator<ESMaskSummonedComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var summoned, out var xform))
        {
            if (summoned.OwnerMind != args.MindId)
                continue;

            if (IsHeld((uid, xform), args.MindId))
                held += 1;
        }

        args.Progress = Math.Clamp((float) held / target, 0, 1);
    }

    private bool IsHeld(Entity<TransformComponent> ent, EntityUid? excludedMind = null)
    {
        foreach (var container in _container.GetContainingContainers(ent.AsNullable()))
        {
            if (_mind.TryGetMind(container.Owner, out var containerMind, out _) && containerMind != excludedMind)
                return true;
        }

        return false;
    }
}
