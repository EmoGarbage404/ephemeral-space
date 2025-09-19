using Content.Server.Maps.NameGenerators;
using JetBrains.Annotations;

namespace Content.Server._ES.Maps.NameGenerators;

[UsedImplicitly]
public sealed partial class ESSimpleNameGenerator : StationNameGenerator
{
    public override string FormatName(string input)
    {
        return input;
    }
}
