using Robust.Shared.GameStates;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESPlantHolderComponent : Component
{
    [DataField]
    public string PlantContainerSlotId = "plant-container-slot";
}
