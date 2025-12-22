using Robust.Shared.GameStates;

namespace Content.Shared._ES.Telesci.Components;

/// <summary>
/// A pad used to spawn rewards for telescience
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ESSharedTelesciSystem))]
public sealed partial class ESTelesciRewardPadComponent : Component
{
    [DataField]
    public bool Enabled = true;
}
