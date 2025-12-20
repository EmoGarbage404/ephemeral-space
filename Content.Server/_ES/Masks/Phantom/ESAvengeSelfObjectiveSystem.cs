using Content.Server._ES.Masks.Objectives.Relays;
using Content.Server._ES.Masks.Objectives.Relays.Components;
using Content.Server._ES.Masks.Phantom.Components;
using Content.Server.KillTracking;
using Content.Shared._ES.Objectives;
using Content.Shared._ES.Objectives.Target;

namespace Content.Server._ES.Masks.Phantom;

public sealed class ESAvengeSelfObjectiveSystem : ESBaseObjectiveSystem<ESAvengeSelfObjectiveComponent>
{
    [Dependency] private readonly ESTargetObjectiveSystem _targetObjective = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    public override Type[] RelayComponents => [typeof(ESKilledRelayComponent)];

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESAvengeSelfObjectiveComponent, ESKillReportedEvent>(OnKillReported);
    }

    private void OnKillReported(Entity<ESAvengeSelfObjectiveComponent> ent, ref ESKillReportedEvent args)
    {
        if (args.Suicide ||
            args.Primary is not KillPlayerSource source ||
            !MindSys.TryGetMind(source.PlayerId, out var mind) ||
            mind.Value.Comp.OwnedEntity is not { } body)
        {
            _metaData.SetEntityName(ent, Loc.GetString(ent.Comp.FailName));
            return;
        }

        _targetObjective.SetTarget(ent.Owner, body);
    }
}
