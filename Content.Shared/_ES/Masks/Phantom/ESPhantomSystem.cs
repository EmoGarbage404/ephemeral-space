using Content.Shared._ES.Masks.Phantom.Components;
using Content.Shared.Actions;
using Content.Shared.Damage.Systems;

namespace Content.Shared._ES.Masks.Phantom;

public sealed class ESPhantomSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESPhantomComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ESPhantomComponent, ESPhantomMaterializeActionEvent>(OnMaterializeAction);

        SubscribeLocalEvent<ESPhantomMaterializedComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<ESPhantomMaterializedComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ESPhantomMaterializedComponent, ComponentRemove>(OnRemove);
    }

    private void OnMapInit(Entity<ESPhantomComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent, ref ent.Comp.ActionEntity, ent.Comp.MaterializeAction);
        Dirty(ent);
    }

    private void OnMaterializeAction(Entity<ESPhantomComponent> ent, ref ESPhantomMaterializeActionEvent args)
    {
        EnsureComp<ESPhantomMaterializedComponent>(ent);
        args.Handled = true;
    }

    private void OnDamageChanged(Entity<ESPhantomMaterializedComponent> ent, ref DamageChangedEvent args)
    {
        if (!args.DamageIncreased)
            return;
        RemCompDeferred<ESPhantomMaterializedComponent>(ent);
    }

    private void OnStartup(Entity<ESPhantomMaterializedComponent> ent, ref ComponentStartup args)
    {
        _appearance.SetData(ent, ESPhantomVisuals.Materialized, true);
    }

    private void OnRemove(Entity<ESPhantomMaterializedComponent> ent, ref ComponentRemove args)
    {
        _appearance.SetData(ent, ESPhantomVisuals.Materialized, false);

        if (TryComp<ESPhantomComponent>(ent, out var comp))
            _actions.SetCooldown(comp.ActionEntity, comp.Cooldown);
    }
}
