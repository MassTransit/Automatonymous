namespace Automatonymous.Behaviors
{
    public interface BehaviorBuilder<TInstance>
    {
        void Add(Activity<TInstance> activity);
    }
}
