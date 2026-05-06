namespace Taskington.Base.TinyBus;

public class SubscriberInfo<F, P>(F subscriber, P? predicate = default)
{
    public F Subscriber { get; } = subscriber;
    public P? Predicate { get; } = predicate;
}
