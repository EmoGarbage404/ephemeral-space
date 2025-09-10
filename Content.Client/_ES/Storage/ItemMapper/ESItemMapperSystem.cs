using Content.Shared._ES.Storage.ItemMapper;
using Content.Shared._ES.Storage.ItemMapper.Components;
using Robust.Client.GameObjects;

namespace Content.Client._ES.Storage.ItemMapper;

public sealed class ESItemMapperSystem : ESSharedItemMapperSystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESItemMapperComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(Entity<ESItemMapperComponent> ent, ref AppearanceChangeEvent args)
    {
        if (args.Sprite is not { } sprite)
            return;

        if (!Appearance.TryGetData(ent, ESItemMapperVisuals.Layers, out Dictionary<string, string?> layers, args.Component))
            return;

        foreach (var (key, state) in layers)
        {
            _sprite.LayerSetVisible((ent, sprite), key, state != null);
            _sprite.LayerSetRsiState((ent, sprite), key, state);
        }
    }
}
