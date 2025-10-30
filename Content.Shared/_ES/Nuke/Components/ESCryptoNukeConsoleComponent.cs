using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._ES.Nuke.Components;

/// <summary>
/// Console that tracks the nuke disk and can be hacked in order to reveal the nuke codes
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause]
[Access(typeof(ESCryptoNukeSystem))]
public sealed partial class ESCryptoNukeConsoleComponent : Component
{
    /// <summary>
    /// Time at which the console UI will update the positions of the disks
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextUpdateTime;

    /// <summary>
    /// Delay between sending UI state updates
    /// </summary>
    [DataField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(2.5f);
}

[Serializable, NetSerializable]
public sealed class ESCryptoNukeConsoleBuiState : BoundUserInterfaceState
{
    public List<NetCoordinates> DiskLocations = new();
}

[Serializable, NetSerializable]
public enum ESCryptoNukeConsoleUiKey : byte
{
    Key,
}
