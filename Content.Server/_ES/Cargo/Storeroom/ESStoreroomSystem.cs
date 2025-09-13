using System.Linq;
using Content.Server.Administration;
using Content.Server.Stack;
using Content.Server.Station.Systems;
using Content.Server.Storage.Components;
using Content.Shared._ES.Cargo.Storeroom.Components;
using Content.Shared.Administration;
using Content.Shared.Whitelist;
using Robust.Shared.Toolshed;

namespace Content.Server._ES.Cargo.Storeroom;

public sealed class ESStoreroomSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly EntityWhitelistSystem _entityWhitelist = default!;
    [Dependency] private readonly StackSystem _stack = default!;
    [Dependency] private readonly StationSystem _station = default!;

    private HashSet<Entity<ESStoreroomPalletComponent, TransformComponent>> _pallets = new();
    private HashSet<EntityUid> _palletGoods = new();

    /// <inheritdoc/>
    public override void Initialize()
    {

    }

    public HashSet<Entity<ESStoreroomPalletComponent, TransformComponent>> GetStoreroomPallets(EntityUid station)
    {
        _pallets.Clear();

        var query = EntityQueryEnumerator<ESStoreroomPalletComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (_station.GetOwningStation(uid, xform) != station)
                continue;

            _pallets.Add((uid, comp, xform));
        }

        return _pallets;
    }

    public Dictionary<ESStoreroomContainerEntry, int> GetStoreroomStock(EntityUid station)
    {
        var containers = new Dictionary<ESStoreroomContainerEntry, int>();
        var processed = new HashSet<EntityUid>(); // put on system

        foreach (var pallet in GetStoreroomPallets(station))
        {
            _palletGoods.Clear();
            _lookup.GetEntitiesIntersecting(
                pallet,
                _palletGoods,
                LookupFlags.Dynamic | LookupFlags.Sundries);

            foreach (var palletGood in _palletGoods)
            {
                if (!processed.Add(palletGood))
                    continue;

                if (_entityWhitelist.IsWhitelistFail(pallet.Comp1.GoodsWhitelist, palletGood))
                    continue;

                var container = CreateContainerEntry(palletGood);
                if (!containers.TryAdd(container, 1))
                    containers[container] += 1;
            }
        }

        return containers;
    }

    private ESStoreroomContainerEntry CreateContainerEntry(EntityUid palletGood)
    {
        var meta = MetaData(palletGood);
        var container = new ESStoreroomContainerEntry(meta.EntityPrototype?.ID, meta.EntityName);

        if (TryComp<EntityStorageComponent>(palletGood, out var entityStorage))
        {
            foreach (var content in entityStorage.Contents.ContainedEntities)
            {
                var contentMeta = MetaData(content);
                if (container.Contents.FirstOrDefault(e =>
                        e.Name.Equals(contentMeta.EntityName, StringComparison.InvariantCultureIgnoreCase))
                    is { } existingEntry)
                {
                    existingEntry.Count += _stack.GetCount(content);
                }
                else
                {
                    var entry = new ESStoreroomEntry(contentMeta.EntityPrototype?.ID, contentMeta.EntityName);
                    entry.Count = _stack.GetCount(content);
                    container.Contents.Add(entry);
                }
            }
        }

        return container;
    }
}

[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed class ESStoreroomCommand : ToolshedCommand
{
    private ESStoreroomSystem? _storeroom;

    [CommandImplementation("stock")]
    public IEnumerable<string> Stock([PipedArgument] EntityUid station)
    {
        _storeroom ??= GetSys<ESStoreroomSystem>();

        yield return "\n";
        foreach (var (container, count) in _storeroom.GetStoreroomStock(station))
        {
            yield return $"{container.Name} x{count}";
            foreach (var content in container.Contents)
            {
                yield return $"\t{content.Name} x{content.Count}";
            }
        }
    }
}
