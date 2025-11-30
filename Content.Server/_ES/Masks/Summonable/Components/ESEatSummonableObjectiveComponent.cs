namespace Content.Server._ES.Masks.Summonable.Components;

[RegisterComponent]
[Access(typeof(ESEatSummonableObjectiveSystem))]
public sealed partial class ESEatSummonableObjectiveComponent : Component
{
    /// <summary>
    /// Number of summonable objects eaten
    /// </summary>
    [DataField]
    public int Eaten;
}
