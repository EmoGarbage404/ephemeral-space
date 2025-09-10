using Content.Shared._ES.Storage.Slots.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nutrition.EntitySystems;

namespace Content.Shared._ES.Storage.Slots;

/// <summary>
/// <see cref="ESOpenableSlotsComponent"/>
/// </summary>
public sealed class ESOpenableSlotSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly OpenableSystem _openable = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESOpenableSlotsComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ESOpenableSlotsComponent, OpenableOpenedEvent>(OnOpened);
        SubscribeLocalEvent<ESOpenableSlotsComponent, OpenableClosedEvent>(OnClosed);
    }

    private void OnMapInit(Entity<ESOpenableSlotsComponent> ent, ref MapInitEvent args)
    {
        UpdateSlotsLocked(ent);
    }

    private void OnOpened(Entity<ESOpenableSlotsComponent> ent, ref OpenableOpenedEvent args)
    {
        UpdateSlotsLocked(ent);
    }

    private void OnClosed(Entity<ESOpenableSlotsComponent> ent, ref OpenableClosedEvent args)
    {
        UpdateSlotsLocked(ent);
    }

    private void UpdateSlotsLocked(Entity<ESOpenableSlotsComponent> ent)
    {
        if (!TryComp<ItemSlotsComponent>(ent, out var slots))
            return;

        var val = !_openable.IsOpen(ent);
        foreach (var slot in ent.Comp.Slots)
        {
            _itemSlots.SetLock(ent, slot, val, slots);
        }
    }
}
