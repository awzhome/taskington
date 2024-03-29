namespace Taskington.Base.TinyBus;

public class SubscriberInfo<F, P>
{
    public F Subscriber { get; }
    public P? Predicate { get; }

    public SubscriberInfo(F subscriber, P? predicate = default)
    {
        Subscriber = subscriber;
        Predicate = predicate;
    }
}
