using Content.Shared._ES.Botany.Hydroponics.Components;

namespace Content.Shared._ES.Botany.Hydroponics;

public abstract partial class ESSharedHydroponicsSystem
{
    public void InitializeHarvest()
    {

    }

    /// <summary>
    /// Checks if a plant is ready to be harvested.
    /// </summary>
    public bool HarvestReady(Entity<ESPlantComponent?, ESPlantHarvestableComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp1, ref ent.Comp2, false))
            return false;

        var plant = ent.Comp1;
        var harvestable = ent.Comp2;

        if (!plant.FullyGrown)
            return false;

        if (_timing.CurTime < harvestable.NextHarvestTime)
            return false;

        return true;
    }

    public bool TryHarvest(Entity<ESPlantComponent?, ESPlantHarvestableComponent?> plant, EntityUid? user)
    {
        if (!Resolve(plant, ref plant.Comp1, ref plant.Comp2, false))
            return false;

        var harvestable = plant.Comp2;

        if (!HarvestReady(plant))
            return false;

        // CHECK FOR TOOLS (if we care about that sort of thing)

        // DO HARVEST LOGIC

        var variance = harvestable.HarvestVariance;
        harvestable.NextHarvestTime = _timing.CurTime + harvestable.HarvestDelay + _random.Next(-variance, variance);
        Appearance.SetData(plant, ESPlantVisuals.Harvest, false);
        return true;
    }
}
