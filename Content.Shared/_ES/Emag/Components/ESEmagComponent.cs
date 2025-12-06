using Robust.Shared.GameStates;

namespace Content.Shared._ES.Emag.Components;

/// <summary>
/// General device that applies emag effects to objects
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(ESEmagSystem))]
public sealed partial class ESEmagComponent : Component;
