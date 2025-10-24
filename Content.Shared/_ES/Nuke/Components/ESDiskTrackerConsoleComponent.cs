using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._ES.Nuke.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause]
[Access(typeof(ESDiskTrackerSystem))]
public sealed partial class ESDiskTrackerConsoleComponent : Component
{
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextUpdateTime;

    [DataField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(5);
}

[Serializable, NetSerializable]
public sealed class ESDiskTrackerConsoleBuiState : BoundUserInterfaceState
{
    public List<NetCoordinates> DiskLocations = new();
}

[Serializable, NetSerializable]
public enum ESDiskTrackerConsoleUiKey : byte
{
    Key,
}
