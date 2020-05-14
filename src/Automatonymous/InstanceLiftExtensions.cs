namespace Automatonymous
{
    using Lifts;


    public static class InstanceLiftExtensions
    {
        public static InstanceLift<T> CreateInstanceLift<T, TInstance>(this T stateMachine, TInstance instance)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class
        {
            var instanceLift = new StateMachineInstanceLift<T, TInstance>(stateMachine, instance);

            return instanceLift;
        }
    }
}
