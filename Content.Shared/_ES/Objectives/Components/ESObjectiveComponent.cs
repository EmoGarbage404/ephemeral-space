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
    /// Current progress on the objective on the interval [0, 1]
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Progress;
}

[ByRefEvent]
public record struct ESGetObjectiveProgressEvent(float Progress = 0);
