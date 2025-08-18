using Content.Shared._ES.Botany.Hydroponics;
using Content.Shared._ES.Botany.Hydroponics.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;

namespace Content.Client._ES.Botany.Hydroponics;

/// <inheritdoc/>
public sealed class ESHydroponicsSystem : ESSharedHydroponicsSystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESPlantHolderComponent, EntInsertedIntoContainerMessage>(OnHolderEntInsertedIntoContainer);
        SubscribeLocalEvent<ESPlantHolderComponent, EntRemovedFromContainerMessage>(OnHolderEntRemovedFromContainer);
        SubscribeLocalEvent<ESPlantComponent, AfterAutoHandleStateEvent>(OnAfterAutoHandleStateEvent);
    }

    protected override void OnHolderStartup(Entity<ESPlantHolderComponent> ent, ref ComponentStartup args)
    {
        base.OnHolderStartup(ent, ref args);

        RefreshHolderSprite((ent, ent));
    }

    private void OnHolderEntInsertedIntoContainer(Entity<ESPlantHolderComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        RefreshHolderSprite((ent, ent));
    }

    private void OnHolderEntRemovedFromContainer(Entity<ESPlantHolderComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        RefreshHolderSprite((ent, ent));
    }

    private void OnAfterAutoHandleStateEvent(Entity<ESPlantComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!Container.TryGetContainingContainer(ent.Owner, out var container) ||
            TerminatingOrDeleted(container.Owner))
            return;
        RefreshHolderSprite(container.Owner);
    }

    public void RefreshHolderSprite(Entity<ESPlantHolderComponent?, SpriteComponent?> holder)
    {
        if (!Resolve(holder, ref holder.Comp1, ref holder.Comp2))
            return;
        var (uid, comp, sprite) = holder;

        if (_sprite.LayerMapTryGet((uid, sprite), ESPlantHolderVisualLayers.Plant, out var idx, true))
        {
            var hasPlant = TryGetPlantFromHolder((uid, comp), out var plant);
            _sprite.LayerSetVisible((uid, sprite), idx, hasPlant);

            if (plant != null)
            {
                if (TryComp<SpriteComponent>(plant.Value, out var plantSprite) &&
                    plantSprite.BaseRSI is { } rsi)
                {
                    _sprite.LayerSetRsi((uid, sprite), idx, rsi);
                }

                var state = $"stage-{plant.Value.Comp.CurrentGrowthStage}";
                _sprite.LayerSetRsiState((uid, sprite), idx, new RSI.StateId(state));
            }
        }
    }
}
