using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.Objectives.Components;

/// <summary>
/// Denotes a general objective that is associated with a <see cref="ESObjectiveHolderComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(ESSharedObjectiveSystem), Other = AccessPermissions.None)]
public sealed partial class ESObjectiveComponent : Component
{
    /// <summary>
    /// Current progress on the objective on the interval [0, <see cref="Target"/>]
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 Progress;

    /// <summary>
    /// Target value <see cref="Progress"/> must equal to qualify the objective as complete.
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 Target = 1;

    /// <summary>
    /// If set, will randomize the value of <see cref="Target"/> on the interval of [<see cref="Target"/>, <see cref="MaxTarget"/>] in increments of <see cref="TargetIncrement"/>
    /// </summary>
    [DataField]
    public FixedPoint2? MaxTarget;

    /// <summary>
    /// Size of "steps" between minimum and maximum target values.
    /// Only used if <see cref="MaxTarget"/> is not null.
    /// </summary>
    [DataField]
    public FixedPoint2 TargetIncrement = 1;
}
