using Content.Server._ES.StationEvents.GasLeak.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Server.StationEvents.Events;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Collections;
using Robust.Shared.Random;

namespace Content.Server._ES.StationEvents.GasLeak;

public sealed class ESGasLeakRule : StationEventSystem<ESGasLeakRuleComponent>
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    protected override void Started(EntityUid uid, ESGasLeakRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        component.NextLeakTime = Timing.CurTime;
        component.LeakGas = RobustRandom.Pick(component.Gasses);
        component.LeakRate = RobustRandom.NextFloat(component.LeakRateRange.X, component.LeakRateRange.Y);
        var moles = RobustRandom.NextFloat(component.LeakMolesRange.X, component.LeakMolesRange.Y);
        Comp<StationEventComponent>(uid).EndTime = Timing.CurTime + TimeSpan.FromSeconds(moles / component.LeakRate) + TimeSpan.FromSeconds(1);

        if (!TryGetRandomStation(out var station))
            return;

        var ventList = new ValueList<Entity<TransformComponent>>();
        var query = EntityQueryEnumerator<GasVentPumpComponent, TransformComponent>();
        while (query.MoveNext(out var ventUid, out _, out var xform))
        {
            if (_station.GetOwningStation(ventUid, xform) == station)
                ventList.Add((ventUid, xform));
        }

        if (ventList.Count == 0)
            return;

        component.LeakOrigin = RobustRandom.Pick(ventList);
    }

    protected override void ActiveTick(EntityUid uid, ESGasLeakRuleComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        if (!Exists(component.LeakOrigin) || TerminatingOrDeleted(component.LeakOrigin))
            return;

        if (Timing.CurTime < component.NextLeakTime)
            return;
        component.NextLeakTime += component.LeakDelay;

        var mixture = _atmosphere.GetTileMixture(component.LeakOrigin);
        mixture?.AdjustMoles(component.LeakGas, (float) (component.LeakRate * component.LeakDelay).TotalSeconds);
    }

    protected override void Ended(EntityUid uid, ESGasLeakRuleComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        if (!RobustRandom.Prob(component.SparkChance))
            return;

        if (!Exists(component.LeakOrigin) || TerminatingOrDeleted(component.LeakOrigin))
            return;

        if (Transform(component.LeakOrigin).GridUid is not { } grid)
            return;

        var indices = _transform.GetGridTilePositionOrDefault(component.LeakOrigin);
        _atmosphere.HotspotExpose(grid, indices, 700f, 50f, null, true);
        Audio.PlayPvs(component.SparkSound, uid);
    }
}
