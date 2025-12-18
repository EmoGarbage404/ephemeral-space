using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Client.Player;

namespace Content.Client._ES.Masks;

public sealed class ESMaskSystem : ESSharedMaskSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESTroupeFactionIconComponent, GetStatusIconsEvent>(OnGetStatusIcons);
    }

    private void OnGetStatusIcons(Entity<ESTroupeFactionIconComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_player.LocalEntity is not { } local)
            return;

        // The main filtering is done on the networking for ESTroupeFactionIconComponent,
        // but this exists largely to catch edge cases where we still have
        // the networked comp on the client even though we shouldn't have access to it.
        if (GetTroupeOrNull(local) != ent.Comp.Troupe)
            return;
        args.StatusIcons.Add(PrototypeManager.Index(ent.Comp.Icon));
    }
}
