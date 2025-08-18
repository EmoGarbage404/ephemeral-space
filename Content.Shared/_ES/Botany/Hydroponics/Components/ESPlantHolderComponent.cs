using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESPlantHolderComponent : Component
{
    [DataField]
    public string PlantContainerSlotId = "plant-container-slot";

    [DataField]
    public EntProtoId<ESPlantComponent>? LastPlanted;
}

public enum ESPlantHolderVisualLayers : byte
{
    Plant,
}
