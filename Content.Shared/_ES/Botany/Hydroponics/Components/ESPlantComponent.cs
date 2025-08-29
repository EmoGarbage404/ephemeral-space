using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

/// <summary>
/// Used for entities which represent plants which are grown inside of <see cref="ESPlantHolderComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true), AutoGenerateComponentPause]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESPlantComponent : Component
{
    #region Watering
    /// <summary>
    /// Whether or not this plant has had water supplied to it, allowing it to grow to the next growth stage.
    /// </summary>
    [ViewVariables]
    public bool Watered => SuppliedWateredReagents >= WateredThreshold;

    /// <summary>
    /// The amount of <see cref="WateredReagents"/> that have already been supplied to this plant
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 SuppliedWateredReagents = 0;

    /// <summary>
    /// The amount of <see cref="WateredReagents"/> that must be supplied for this plant to grow
    /// </summary>
    [DataField]
    public FixedPoint2 WateredThreshold = 15;

    /// <summary>
    /// Reagents which enable the <see cref="Watered"/> state.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<ReagentPrototype>> WateredReagents = new();
    #endregion

    /// <summary>
    /// If the plant has completed all of their growth stages
    /// </summary>
    [ViewVariables]
    public bool FullyGrown => CurrentGrowthStage >= GrowthStages;

    /// <summary>
    /// How much this plant has grown so far
    /// </summary>
    [DataField, AutoNetworkedField]
    public int CurrentGrowthStage = 1;

    /// <summary>
    /// The number of stages this plant has to grow before <see cref="FullyGrown"/> is true
    /// </summary>
    [DataField(required: true)]
    public int GrowthStages;

    /// <summary>
    /// Time at which the plant will advance a growth stage.
    /// </summary>
    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextGrowthTime;

    /// <summary>
    /// Base amount of time between growth stages
    /// </summary>
    [DataField]
    public TimeSpan GrowthStageDuration = TimeSpan.FromSeconds(120);

    /// <summary>
    /// Width of uniform variance applied to <see cref="GrowthStageVariance"/>
    /// </summary>
    [DataField]
    public TimeSpan GrowthStageVariance = TimeSpan.FromSeconds(15);
}

[Serializable, NetSerializable]
public enum ESPlantVisuals : byte
{
    GrowthStage,
    Harvest,
    Dead,
}
