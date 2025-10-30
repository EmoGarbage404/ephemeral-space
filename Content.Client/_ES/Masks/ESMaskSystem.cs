using Content.Shared._ES.Masks;
using Content.Shared._ES.Masks.Components;
using Content.Shared.StatusIcon.Components;

namespace Content.Client._ES.Masks;

public sealed class ESMaskSystem : ESSharedMaskSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESTroupeFactionIconComponent, GetStatusIconsEvent>(OnGetStatusIcons);
    }

    private void OnGetStatusIcons(Entity<ESTroupeFactionIconComponent> ent, ref GetStatusIconsEvent args)
    {
        args.StatusIcons.Add(PrototypeManager.Index(ent.Comp.Icon));
    }
}
