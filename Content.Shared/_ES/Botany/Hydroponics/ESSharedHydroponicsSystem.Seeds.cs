using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Botany.Hydroponics.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;

namespace Content.Shared._ES.Botany.Hydroponics;

public sealed partial class ESSharedHydroponicsSystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public void InitializeSeeds()
    {
        SubscribeLocalEvent<ESSeedComponent, AfterInteractEvent>(OnSeedAfterInteract);
    }

    private void OnSeedAfterInteract(Entity<ESSeedComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<ESPlantHolderComponent>(args.Target, out var plantHolderComp))
            return;

        args.Handled = true;

        if (TryPlantSeed(ent.AsNullable(), (args.Target.Value, plantHolderComp), out _, args.User))
        {
            // Delete the seed if we successfully plant it
            PredictedQueueDel(ent.Owner);
        }
    }

    public bool TryPlantSeed(Entity<ESSeedComponent?> seed,
        Entity<ESPlantHolderComponent?> plantHolder,
        [NotNullWhen(true)] out Entity<ESPlantComponent>? plant,
        EntityUid? user = null)
    {
        plant = null;

        if (!Resolve(seed, ref seed.Comp) || !Resolve(plantHolder, ref plantHolder.Comp))
            return false;

        if (HolderHasPlant(plantHolder.AsNullable()))
        {
            if (user != null)
                _popup.PopupClient(Loc.GetString("es-hydroponics-tray-popup-already-planted"), user.Value, user);
            return false;
        }

        if (!_container.TryGetContainer(plantHolder, plantHolder.Comp.PlantContainerSlotId, out var container))
            return false;

        var plantUid = Spawn(seed.Comp.Plant);
        var plantComp = EnsureComp<ESPlantComponent>(plantUid);
        plant = (plantUid, plantComp);

        _container.Insert(plantUid, container, force: true);

        if (user != null)
            _popup.PopupClient(Loc.GetString("es-hydroponics-tray-popup-planted", ("plant", seed.Owner)), user.Value, user);

        return true;
    }
}
