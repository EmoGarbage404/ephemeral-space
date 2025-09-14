using Robust.Shared.GameStates;

namespace Content.Shared._ES.Cargo.Storeroom.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESStoreroomStationComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<ESStoreroomContainerEntry, int> Stock = new();
}
