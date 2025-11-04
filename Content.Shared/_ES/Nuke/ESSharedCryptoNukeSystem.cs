using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Masks;
using Content.Shared._ES.Nuke.Components;
using Content.Shared.Examine;
using Content.Shared.Mind;
using Content.Shared.Objectives.Systems;
using Content.Shared.Station;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._ES.Nuke;

public abstract class ESSharedCryptoNukeSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly ESSharedMaskSystem _mask = default!;
    [Dependency] private readonly SharedObjectivesSystem _objectives = default!;
    [Dependency] protected readonly SharedStationSystem Station = default!;
    [Dependency] protected readonly SharedUserInterfaceSystem UserInterface = default!;

    public static readonly ProtoId<ESTroupePrototype> TraitorTroupe = "ESTraitor";

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

    /// <summary>
    /// Checks if a given entity is capable of hacking the terminal
    /// </summary>
    public bool ArePreRequisiteObjectivesDone()
    {
        if (!_mask.TryGetTroupeEntity(TraitorTroupe, out var troupe) ||
            !TryComp<MindComponent>(troupe, out var mind))
            return false;

        foreach (var objective in troupe.Value.Comp.AssociatedObjectives)
        {
            if (!HasComp<ESNukePrereqObjectiveComponent>(objective))
                continue;

            if (_objectives.IsCompleted(objective, (troupe.Value, mind)))
                continue;

            return false;
        }

        return true;
    }
}
