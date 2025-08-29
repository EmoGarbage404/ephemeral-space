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
        SubscribeLocalEvent<ESPlantComponent, AppearanceChangeEvent>(OnPlantAppearanceChange);
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

    private void OnPlantAppearanceChange(Entity<ESPlantComponent> ent, ref AppearanceChangeEvent args)
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

        _sprite.LayerSetVisible((uid, sprite), ESPlantHolderVisualLayers.Plant, HolderHasPlant((uid, comp)));

        if (TryGetPlantFromHolder((uid, comp), out var plant))
        {
            _sprite.LayerSetRsi((uid, sprite), ESPlantHolderVisualLayers.Plant, CompOrNull<SpriteComponent>(plant.Value)?.BaseRSI);

            var state = "stage-1";
            if (Appearance.TryGetData<bool>(plant.Value, ESPlantVisuals.Dead, out var dead) && dead)
                state = "dead";
            if (Appearance.TryGetData<bool>(plant.Value, ESPlantVisuals.Harvest, out var harvest) && harvest)
                state = "harvest";
            else if (Appearance.TryGetData<int>(plant.Value, ESPlantVisuals.GrowthStage, out var stage))
                state = $"stage-{stage}";

            _sprite.LayerSetRsiState((uid, sprite), ESPlantHolderVisualLayers.Plant, new RSI.StateId(state));
        }
    }
}
