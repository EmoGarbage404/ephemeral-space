using System.Diagnostics.CodeAnalysis;
using Content.Shared._ES.Botany.Hydroponics.Components;
using Content.Shared.Interaction;

namespace Content.Shared._ES.Botany.Hydroponics;

public abstract partial class ESSharedHydroponicsSystem
{
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

        TryPlantSeed(ent.AsNullable(), (args.Target.Value, plantHolderComp), out _, args.User);
        args.Handled = true; // Always handle the event, as we display popups that we don't want to overlap.
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

        if (!Container.TryGetContainer(plantHolder, plantHolder.Comp.PlantContainerSlotId, out var container))
            return false;

        var plantUid = Spawn(seed.Comp.Plant);
        var plantComp = EnsureComp<ESPlantComponent>(plantUid);
        plant = (plantUid, plantComp);

        Container.Insert(plantUid, container, force: true);
        Dirty(plantUid, plantComp);

        plantHolder.Comp.LastPlanted = seed.Comp.Plant;

        if (user != null)
            _popup.PopupClient(Loc.GetString("es-hydroponics-tray-popup-planted", ("plant", seed.Owner)), user.Value, user);

        // Delete the seed if we successfully plant it
        PredictedQueueDel(seed.Owner);
        return true;
    }
}
