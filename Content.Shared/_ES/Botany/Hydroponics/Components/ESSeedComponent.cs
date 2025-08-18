using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESSeedComponent : Component
{
    /// <summary>
    /// The plant that this seed grows.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId<ESPlantComponent> Plant;
}
