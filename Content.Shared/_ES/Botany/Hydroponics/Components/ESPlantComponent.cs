using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESPlantComponent : Component
{
    #region Watering
    /// <summary>
    /// Whether or not this plant has had water supplied to it, allowing it to grow to the next growth stage.
    /// </summary>
    public bool Watered => SuppliedWateredReagents >= WateredThreshold;

    /// <summary>
    /// The amount of <see cref="WateredReagents"/> that have already been supplied to this plant
    /// </summary>
    [DataField]
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

    [DataField]
    public int CurrentGrowthStage = 1;

    [DataField]
    public int GrowthStages;

    [DataField]
    public TimeSpan GrowthStageDuration = TimeSpan.FromSeconds(120);
}
