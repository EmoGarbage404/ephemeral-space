using Content.Shared.Actions.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ES.Actions.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(ESActionSystem))]
public sealed partial class ESInherentActionsComponent : Component
{
    [DataField]
    public List<EntProtoId<ActionComponent>> Actions = new();

    [DataField]
    public List<EntityUid> ActionEntities = new();
}
