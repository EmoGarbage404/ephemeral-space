using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Nuke.Components;
using Content.Shared.Examine;
using Content.Shared.Station;
using Robust.Shared.Timing;

namespace Content.Shared._ES.Nuke;

public abstract class ESSharedCryptoNukeSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] protected readonly SharedStationSystem Station = default!;
    [Dependency] protected readonly SharedUserInterfaceSystem UserInterface = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESCryptoNukeConsoleComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ESCryptoNukeConsoleComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<ESCryptoNukeConsoleComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange || !ent.Comp.Compromised)
            return;
        args.PushMarkup(Loc.GetString("es-cryptonuke-examine-compromised"));
    }

    private void OnMapInit(Entity<ESCryptoNukeConsoleComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextUpdateTime = Timing.CurTime;
    }

    /// <summary>
    /// Checks all consoles on a station to see if they are all compromised.
    /// </summary>
    public bool IsStationCompromised([NotNullWhen(true)] EntityUid? station)
    {
        if (station is null)
            return false;

        var query = EntityQueryEnumerator<ESCryptoNukeConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (Station.GetOwningStation(uid, xform) != station)
                continue;

            // Exit early if we find a single compromised consoles.
            if (!comp.Compromised)
                return false;
        }

        return true;
    }
}
