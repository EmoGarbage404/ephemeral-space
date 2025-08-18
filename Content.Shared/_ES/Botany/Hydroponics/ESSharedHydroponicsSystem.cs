using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Botany.Hydroponics.Components;
using Robust.Shared.Containers;

namespace Content.Shared._ES.Botany.Hydroponics;

public sealed partial class ESSharedHydroponicsSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        InitializeSeeds();

        SubscribeLocalEvent<ESPlantHolderComponent, ComponentStartup>(OnHolderStartup);
    }

    private void OnHolderStartup(Entity<ESPlantHolderComponent> ent, ref ComponentStartup args)
    {
        _container.EnsureContainer<ContainerSlot>(ent, ent.Comp.PlantContainerSlotId);
    }

    public bool TryGetPlantFromHolder(Entity<ESPlantHolderComponent?> ent, [NotNullWhen(true)] out Entity<ESPlantComponent>? plant)
    {
        plant = null;

        if (!Resolve(ent, ref ent.Comp))
            return false;

        if (!_container.TryGetContainer(ent, ent.Comp.PlantContainerSlotId, out var container))
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
}
