using Content.Shared._ES.Objectives.Components;

namespace Content.Shared._ES.Objectives;

public abstract partial class ESSharedObjectiveSystem
{
    private void InitializeCounter()
    {
        SubscribeLocalEvent<ESCounterObjectiveComponent, MapInitEvent>(OnCounterMapInit);
        SubscribeLocalEvent<ESCounterObjectiveComponent, ESGetObjectiveProgressEvent>(OnCounterGetProgress);
    }

    private void OnCounterMapInit(Entity<ESCounterObjectiveComponent> ent, ref MapInitEvent args)
    {
        // No variation in target, no further logic needed
        if (ent.Comp.MaxTarget is not { } maxTarget)
            return;

        // Generate a random value on [target, maxTarget] in chunks of targetIncrement
        var range = maxTarget - ent.Comp.Target;
        var incrementCount = (int) Math.Ceiling(range / ent.Comp.TargetIncrement);
        var blend = _random.Next(0, incrementCount + 1); // non-inclusive right bound adjustment
        ent.Comp.Target = Math.Clamp(ent.Comp.Target + blend * ent.Comp.TargetIncrement, ent.Comp.Target, maxTarget);
        Dirty(ent);
    }

    private void OnCounterGetProgress(Entity<ESCounterObjectiveComponent> ent, ref ESGetObjectiveProgressEvent args)
    {
        args.Progress = ent.Comp.Counter / ent.Comp.Target;
    }

    /// <summary>
    /// Adjusts the counter for the objective by <see cref="val"/>
    /// </summary>
    /// <param name="ent">Objective entity</param>
    /// <param name="val">How much to add or remove from the counter</param>
    public void AdjustObjectiveCounter(Entity<ESObjectiveComponent?, ESCounterObjectiveComponent?> ent, float val)
    {
        if (!Resolve(ent, ref ent.Comp1, ref ent.Comp2))
            return;

        // Don't allow counters to go into the negatives.
        ent.Comp2.Counter = Math.Max(ent.Comp2.Counter + val, 0f);
        Dirty(ent, ent.Comp2);

        RefreshObjectiveProgress((ent, ent));
    }
}
