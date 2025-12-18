using Content.Shared._ES.Sparks.Components;
using Content.Shared.Damage.Systems;

namespace Content.Shared._ES.Sparks;

public sealed partial class ESSparksSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESSparkOnHitComponent, DamageChangedEvent>(OnDamaged);
    }

    private void OnDamaged(Entity<ESSparkOnHitComponent> ent, ref DamageChangedEvent args)
    {
        if (args.DamageDelta is null)
            return;

        if (args.DamageDelta.GetTotal() < ent.Comp.Threshold)
            return;

        DoSparks(ent.AsNullable(), user: args.Origin);
    }
}
