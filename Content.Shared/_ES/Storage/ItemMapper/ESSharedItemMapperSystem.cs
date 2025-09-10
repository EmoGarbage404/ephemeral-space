using Content.Shared._ES.Storage.ItemMapper.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;

namespace Content.Shared._ES.Storage.ItemMapper;

public abstract class ESSharedItemMapperSystem : EntitySystem
{
    [Dependency] protected readonly SharedAppearanceSystem Appearance = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly EntityWhitelistSystem _entityWhitelist = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESItemMapperComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ESItemMapperComponent, EntInsertedIntoContainerMessage>(OnEntInserted);
        SubscribeLocalEvent<ESItemMapperComponent, EntRemovedFromContainerMessage>(OnEntRemoved);
    }

    private void OnStartup(Entity<ESItemMapperComponent> ent, ref ComponentStartup args)
    {
        UpdateMappings(ent);
    }

    private void OnEntInserted(Entity<ESItemMapperComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        UpdateMappings(ent);
    }

    private void OnEntRemoved(Entity<ESItemMapperComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        UpdateMappings(ent);
    }

    public void UpdateMappings(Entity<ESItemMapperComponent> ent)
    {
        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        var layers = new Dictionary<string, string?>();

        foreach (var (layerKey, mappings) in ent.Comp.Mappings)
        {
            string? layerState = null;
            foreach (var mapping in mappings)
            {
                if (!IsMappingSatisfied((ent, ent, containerManager), mapping))
                    continue;
                layerState = mapping.State;
                break; // Exit on the first valid mapping.
            }

            layers.Add(layerKey, layerState);
        }

        Appearance.SetData(ent, ESItemMapperVisuals.Layers, layers);
    }

    private bool IsMappingSatisfied(Entity<ESItemMapperComponent, ContainerManagerComponent> ent, ESItemLayerMapping mapping)
    {
        if (!_container.TryGetContainer(ent, mapping.ContainerId, out var container, ent))
            return false;

        var count = 0;
        foreach (var containedEntity in container.ContainedEntities)
        {
            if (_entityWhitelist.IsWhitelistPassOrNull(mapping.Whitelist, containedEntity))
                count++;
        }

        return mapping.Range.Contains(count);
    }
}
