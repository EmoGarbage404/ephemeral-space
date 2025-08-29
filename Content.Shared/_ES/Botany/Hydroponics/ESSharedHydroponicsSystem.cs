using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._ES.Botany.Hydroponics.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._ES.Botany.Hydroponics;

public abstract partial class ESSharedHydroponicsSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] protected readonly SharedAppearanceSystem Appearance = default!;
    [Dependency] protected readonly SharedContainerSystem Container = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        InitializeHarvest();
        InitializeSeeds();

        SubscribeLocalEvent<ESPlantHolderComponent, ComponentStartup>(OnHolderStartup);
        SubscribeLocalEvent<ESPlantHolderComponent, SolutionTransferredEvent>(OnHolderSolutionTransferred);
        SubscribeLocalEvent<ESPlantHolderComponent, ExaminedEvent>(OnExamined);

        SubscribeLocalEvent<ESPlantComponent, MapInitEvent>(OnPlantMapInit);
    }

    protected virtual void OnHolderStartup(Entity<ESPlantHolderComponent> ent, ref ComponentStartup args)
    {
        Container.EnsureContainer<ContainerSlot>(ent, ent.Comp.PlantContainerSlotId);
    }

    // TODO: prevent solution transfers without plants, i think? idk.
    private void OnHolderSolutionTransferred(Entity<ESPlantHolderComponent> ent, ref SolutionTransferredEvent args)
    {
        if (!_solutionContainer.TryGetRefillableSolution(ent.Owner, out _, out var solution))
            return;

        if (!TryGetPlantFromHolder(ent.AsNullable(), out var plant))
            return;
        var plantComp = plant.Value.Comp;

        var soln = solution.SplitSolutionWithOnly(solution.Volume, plantComp.WateredReagents.Select(p => p.ToString()).ToArray());
        plantComp.SuppliedWateredReagents += soln.Volume;

        // If we were just waiting on water to grow the plant,
        // Make it less obvious by delaying the growth a few seconds.
        if (_timing.CurTime >= plantComp.NextGrowthTime)
            plantComp.NextGrowthTime = _timing.CurTime + _random.Next(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));

        Dirty(plant.Value);
    }

    private void OnExamined(Entity<ESPlantHolderComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(ESPlantHolderComponent)))
        {
            var hasPlant = TryGetPlantFromHolder(ent.AsNullable(), out var plant);

            args.PushMarkup(hasPlant
                ? Loc.GetString("es-hydroponics-tray-examine-filled", ("holder", ent), ("plant", plant!))
                : Loc.GetString("es-hydroponics-tray-examine-empty", ("holder", ent)));

            // TODO: might be cleaner to just relay the event to the plant itself
            if (hasPlant)
            {
                if (plant!.Value.Comp.Watered)
                {
                    args.PushMarkup(Loc.GetString("es-hydroponics-tray-examine-watered-watered", ("plant", plant.Value)));
                }
                else if (plant.Value.Comp.SuppliedWateredReagents > 0)
                {
                    args.PushMarkup(Loc.GetString("es-hydroponics-tray-examine-watered-partial", ("plant", plant.Value)));
                }
                else
                {
                    args.PushMarkup(Loc.GetString("es-hydroponics-tray-examine-watered-none", ("plant", plant.Value)));
                }
            }
        }
    }

    private void OnPlantMapInit(Entity<ESPlantComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextGrowthTime = _timing.CurTime
                                  + ent.Comp.GrowthStageDuration
                                  + _random.Next(-ent.Comp.GrowthStageVariance, ent.Comp.GrowthStageVariance);
        Dirty(ent);
    }

    public bool TryGetPlantFromHolder(Entity<ESPlantHolderComponent?> ent, [NotNullWhen(true)] out Entity<ESPlantComponent>? plant)
    {
        plant = null;

        if (!Resolve(ent, ref ent.Comp))
            return false;

        if (!Container.TryGetContainer(ent, ent.Comp.PlantContainerSlotId, out var container))
            return false;

        if (container is not ContainerSlot { ContainedEntity: { } plantUid })
            return false;

        if (!TryComp<ESPlantComponent>(plantUid, out var plantComp))
            return false;

        plant = (plantUid, plantComp);
        return true;
    }

    public bool HolderHasPlant(Entity<ESPlantHolderComponent?> ent)
    {
        return TryGetPlantFromHolder(ent, out _);
    }

    // TODO: move into partial class
    #region Plant subsystem
    private void UpdatePlant(Entity<ESPlantComponent> ent)
    {
        if (!ent.Comp.FullyGrown && _timing.CurTime >= ent.Comp.NextGrowthTime)
        {
            TryAdvanceGrowthStage(ent);
        }
    }

    public bool TryAdvanceGrowthStage(Entity<ESPlantComponent> ent)
    {
        if (!CanAdvanceGrowthStage(ent))
            return false;

        ent.Comp.CurrentGrowthStage = Math.Min(ent.Comp.CurrentGrowthStage + 1, ent.Comp.GrowthStages);

        ent.Comp.SuppliedWateredReagents = 0;
        ent.Comp.NextGrowthTime = _timing.CurTime
                                  + ent.Comp.GrowthStageDuration
                                  + _random.Next(-ent.Comp.GrowthStageVariance, ent.Comp.GrowthStageVariance);
        Dirty(ent);
        return true;
    }

    public bool CanAdvanceGrowthStage(Entity<ESPlantComponent> ent)
    {
        if (!ent.Comp.Watered)
            return false;

        // Check compost here.

        return true;
    }

    #endregion

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var plantQuery = EntityQueryEnumerator<ESPlantComponent>();
        while (plantQuery.MoveNext(out var uid, out var comp))
        {
            UpdatePlant((uid, comp));
        }

        var harvestableQuery = EntityQueryEnumerator<ESPlantHarvestableComponent, ESPlantComponent, AppearanceComponent>();
        while (harvestableQuery.MoveNext(out var uid, out var comp1, out var comp2, out var comp3))
        {
            if (!HarvestReady((uid, comp2, comp1)))
                continue;
            Appearance.SetData(uid, ESPlantVisuals.Harvest, true, comp3);
        }
    }
}
