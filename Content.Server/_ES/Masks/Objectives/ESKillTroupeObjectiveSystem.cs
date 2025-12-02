using Content.Server._ES.Masks.Objectives.Components;
using Content.Server.KillTracking;
using Content.Server.Mind;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server._ES.Masks.Objectives;

/// <summary>
/// This handles <see cref="ESKillTroupeObjectiveComponent"/>
/// </summary>
public sealed class ESKillTroupeObjectiveSystem : EntitySystem
{
    [Dependency] private readonly ESMaskSystem _mask = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESKillTroupeObjectiveComponent, ObjectiveGetProgressEvent>(OnGetProgress);
        SubscribeLocalEvent<KillReportedEvent>(OnKillReported);
    }

    private void OnGetProgress(Entity<ESKillTroupeObjectiveComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target == 0)
            return;
        args.Progress = Math.Clamp((float) ent.Comp.Kills / target, 0, 1);
    }

    private void OnKillReported(ref KillReportedEvent args)
    {
        if (args.Primary is not KillPlayerSource source ||
            !_mind.TryGetMind(source.PlayerId, out var mind))
            return;

        foreach (var objective in _mind.ESGetObjectivesComp<ESKillTroupeObjectiveComponent>(mind.Value.AsNullable()))
        {
            if (!_mask.TryGetTroupe(args.Entity, out var troupe))
                return;

            if ((troupe == objective.Comp.Troupe) ^ objective.Comp.Invert)
                objective.Comp.Kills += 1;
        }
    }
}
