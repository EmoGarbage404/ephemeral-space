using Content.Shared._ES.Stagehand.Components;
using Robust.Shared.Player;

namespace Content.Shared._ES.Stagehand;

public sealed class ESUsernameEntityNameSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESUsernameEntityNameComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ESUsernameEntityNameComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<ESUsernameEntityNameComponent, PlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnStartup(Entity<ESUsernameEntityNameComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.OriginalName = Name(ent);
        Dirty(ent);
    }

    private void OnPlayerAttached(Entity<ESUsernameEntityNameComponent> ent, ref PlayerAttachedEvent args)
    {
        _metaData.SetEntityName(ent, args.Player.Name);
    }

    private void OnPlayerDetached(Entity<ESUsernameEntityNameComponent> ent, ref PlayerDetachedEvent args)
    {
        _metaData.SetEntityName(ent, ent.Comp.OriginalName);
    }
}
