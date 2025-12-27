using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._ES.Telesci.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(ESSharedTelesciSystem))]
public sealed partial class ESPortalGeneratorComponent : Component
{
    [DataField]
    public TimeSpan UpdateDelay = TimeSpan.FromSeconds(1);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan NextUpdateTime;

    [DataField, AutoNetworkedField]
    public TimeSpan AccumulatedChargeTime = TimeSpan.Zero;

    [DataField]
    public TimeSpan ChargeDuration = TimeSpan.FromMinutes(12.5f);

    [ViewVariables]
    public bool Charged => AccumulatedChargeTime > ChargeDuration;

    [DataField, AutoNetworkedField]
    public bool Powered;
}

[Serializable, NetSerializable]
public enum ESPortalGeneratorVisuals : byte
{
    Charged,
}
