namespace Automatonymous.Graphing
{
    public static class GraphStateMachineExtensions
    {
        public static StateMachineGraph GetGraph<TInstance>(this StateMachine<TInstance> machine)
            where TInstance : class
        {
            var inspector = new GraphStateMachineVisitor<TInstance>();

            machine.Accept(inspector);

            return inspector.Graph;
        }
    }
}
