using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.Telesci.Components;

/// <summary>
/// Marks a station as supporting telescience research and its related objectives.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(ESSharedTelesciSystem))]
public sealed partial class ESTelesciStationComponent : Component
{
    [DataField, AutoNetworkedField]
    public int Stage;

    [ViewVariables]
    public int MaxStage => Stages.Count;

    [DataField]
    public List<ESTelesciStage> Stages = [];

    [DataField]
    public int RewardPads = 4;
}

[DataDefinition]
public partial struct ESTelesciStage
{
    [DataField]
    public int Danger;

    [DataField]
    public EntityTableSelector Rewards = new NoneSelector();

    [DataField]
    public EntityTableSelector Events = new NoneSelector();
}
