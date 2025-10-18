using Content.Shared._ES.Actions.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;

namespace Content.Shared._ES.Actions;

public sealed class ESActionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ESInherentActionsComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<ESInherentActionsComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<ActionsComponent>(ent, out var actions))
            return;

        foreach (var id in ent.Comp.Actions)
        {
            if (_actions.AddAction(ent, id, component: actions) is { } action)
                ent.Comp.ActionEntities.Add(action);
        }
    }
}
