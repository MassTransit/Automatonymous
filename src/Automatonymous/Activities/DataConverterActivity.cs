// Copyright 2011-2016 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Activities
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class DataConverterActivity<TInstance, TData> :
        Activity<TInstance>
    {
        readonly Activity<TInstance, TData> _activity;

        public DataConverterActivity(Activity<TInstance, TData> activity)
        {
            _activity = activity;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x => _activity.Accept(visitor));
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            throw new AutomatonymousException("This activity requires a body with the event, but no body was specified.");
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var dataContext = context as BehaviorContext<TInstance, TData>;
            if (dataContext == null)
                throw new AutomatonymousException("Expected Type " + typeof(TData).Name + " but was " + context.Data.GetType().Name);

            var dataNext = next as Behavior<TInstance, TData>;
            if (dataNext == null)
                throw new AutomatonymousException("The next behavior was not a valid type");

            return _activity.Execute(dataContext, dataNext);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context,
            Behavior<TInstance, T> next)
        {
            var dataContext = context as BehaviorExceptionContext<TInstance, TData, TException>;
            if (dataContext == null)
                throw new AutomatonymousException("Expected Type " + typeof(TData).Name + " but was " + context.Data.GetType().Name);

            var dataNext = next as Behavior<TInstance, TData>;
            if (dataNext == null)
                throw new AutomatonymousException("The next behavior was not a valid type");

            return _activity.Faulted(dataContext, dataNext);
        }
    }
}