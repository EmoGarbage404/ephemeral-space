namespace Content.Server._ES.Masks.Phantom.Components;

/// <summary>
/// Used to set up a new target for a given objective when the objective's owner gets killed.
/// </summary>
[RegisterComponent]
[Access(typeof(ESAvengeSelfObjectiveSystem))]
public sealed partial class ESAvengeSelfObjectiveComponent : Component
{
    [DataField]
    public LocId FailName = "es-phantom-avenge-objective-fail";
}
