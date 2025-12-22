using Content.Server.Administration;
using Content.Shared._ES.Telesci;
using Content.Shared.Administration;
using Robust.Shared.Toolshed;

namespace Content.Server._ES.Telesci;

public sealed class ESTelesciSystem : ESSharedTelesciSystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
    }
}

[ToolshedCommand, AdminCommand(AdminFlags.Round)]
public sealed class ESTelesciCommand : ToolshedCommand
{
    private ESTelesciSystem? _telesci;

    [CommandImplementation("advanceStage")]
    public void AdvanceStage([PipedArgument] EntityUid station)
    {
        _telesci = Sys<ESTelesciSystem>();
        _telesci.AdvanceTelesciStage(station);
    }

    [CommandImplementation("setStage")]
    public void SetStage([PipedArgument] EntityUid station, int stage)
    {
        _telesci = Sys<ESTelesciSystem>();
        _telesci.SetTelesciStage(station, stage);
    }
}
