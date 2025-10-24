using Content.Shared._ES.Nuke.Components;
using Content.Shared.Nuke;
using Robust.Shared.Timing;

namespace Content.Shared._ES.Nuke;

public sealed class ESDiskTrackerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _userInterface = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESDiskTrackerConsoleComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<ESDiskTrackerConsoleComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextUpdateTime = _timing.CurTime;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ESDiskTrackerConsoleComponent, UserInterfaceComponent>();
        while (query.MoveNext(out var uid, out var tracker, out var ui))
        {
            if (_timing.CurTime < tracker.NextUpdateTime)
                continue;
            tracker.NextUpdateTime += tracker.UpdateRate;

            var state = new ESDiskTrackerConsoleBuiState();
            var diskQuery = EntityQueryEnumerator<NukeDiskComponent, TransformComponent>();
            while (diskQuery.MoveNext(out _, out _, out var xform))
            {
                state.DiskLocations.Add(GetNetCoordinates(xform.Coordinates));
            }

            _userInterface.SetUiState((uid, ui), ESDiskTrackerConsoleUiKey.Key, state);
        }
    }
}
