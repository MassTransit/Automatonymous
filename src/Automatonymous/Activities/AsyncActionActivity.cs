namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;


    public class AsyncActionActivity<TInstance>
        : Activity<TInstance>
        where TInstance : class
    {
        readonly Func<TInstance, Task> _func;

        public AsyncActionActivity(Func<TInstance, Task> func)
        {
            _func = func;
        }

        public void Accept(StateMachineInspector inspector)
        {
            throw new NotImplementedException();
        }

        public void Execute(TInstance instance)
        {
            throw new NotImplementedException();
        }

        public void Execute<TData>(TInstance instance, TData value)
        {
            throw new NotImplementedException();
        }
    }
}