using Robust.Shared.GameStates;

namespace Content.Shared._ES.Botany.Hydroponics.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(ESSharedHydroponicsSystem))]
public sealed partial class ESPlantHarvestableComponent : Component
{
    /// <summary>
    /// The time at which the plant will become harvestable
    /// </summary>
    /// <remarks>
    /// Note that this value is kept at 0. This means that as the plant is fully grown,
    /// it will be able to be harvested.
    /// </remarks>
    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextHarvestTime;

    /// <summary>
    /// The time between harvests. In effect, how long does it take for
    /// a plant to become harvestable again after being harvested.
    /// </summary>
    [DataField]
    public TimeSpan HarvestDelay = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Width of uniform variance applied to <see cref="HarvestDelay"/>
    /// </summary>
    [DataField]
    public TimeSpan HarvestVariance = TimeSpan.FromSeconds(15);
}
