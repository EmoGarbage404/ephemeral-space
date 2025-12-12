using Robust.Shared.GameStates;
using Robust.Shared.Utility;

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

    /// <summary>
    /// Icon displayed for this objective in the UI.
    /// </summary>
    [DataField]
    public SpriteSpecifier? Icon;
}

[ByRefEvent]
public record struct ESGetObjectiveProgressEvent(float Progress = 0);

[ByRefEvent]
public readonly record struct ESInitializeObjectiveEvent;
