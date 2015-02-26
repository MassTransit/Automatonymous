// Copyright 2011-2015 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Accessors
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class RawStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateMachine<TInstance> _machine;
        readonly IObserver<StateChanged<TInstance>> _observer;
        readonly ReadWriteProperty<TInstance, State> _property;

        public RawStateAccessor(StateMachine<TInstance> machine, Expression<Func<TInstance, State>> currentStateExpression,
            IObserver<StateChanged<TInstance>> observer)
        {
            _machine = machine;
            _observer = observer;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        async Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            State state = _property.Get(context.Instance);
            if (state == null)
                return null;

            return _machine.GetState(state.Name);
        }

        async Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            State previous = _property.Get(context.Instance);
            if (state.Equals(previous))
                return;

            _property.Set(context.Instance, state);

            State<TInstance> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous.Name);

            _observer.OnNext(new StateChangedEvent(context.Instance, previousState, state));
        }

        static ReadWriteProperty<TInstance, State> GetCurrentStateProperty(Expression<Func<TInstance, State>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, State>(propertyInfo, true);
        }


        class StateChangedEvent :
            StateChanged<TInstance>
        {
            public StateChangedEvent(TInstance instance, State previous, State current)
            {
                Instance = instance;
                Previous = previous;
                Current = current;
            }

            public TInstance Instance { get; private set; }

            public State Previous { get; private set; }

            public State Current { get; private set; }
        }
    }
}