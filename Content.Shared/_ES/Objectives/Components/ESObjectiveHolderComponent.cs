using Robust.Shared.GameStates;

namespace Content.Shared._ES.Objectives.Components;

/// <summary>
/// Denotes an entity that can have objectives associated with it.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(ESSharedObjectiveSystem), Other = AccessPermissions.None)]
public sealed partial class ESObjectiveHolderComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntityUid> Objectives = [];
}
