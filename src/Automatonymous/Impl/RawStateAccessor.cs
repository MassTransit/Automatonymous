// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Impl
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;
    using Internals.Reflection;


    public class RawStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly IObserver<StateChanged<TInstance>> _observer;
        readonly ReadWriteProperty<TInstance, State> _property;

        public RawStateAccessor(Expression<Func<TInstance, State>> currentStateExpression,
                                IObserver<StateChanged<TInstance>> observer)
        {
            _observer = observer;
            PropertyInfo statePropertyInfo = currentStateExpression.GetPropertyInfo();

            _property = new ReadWriteProperty<TInstance, State>(statePropertyInfo, true);
        }

        public State<TInstance> Get(TInstance instance)
        {
            return _property.Get(instance) as State<TInstance>;
        }

        public void Set(TInstance instance, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            State<TInstance> previous = Get(instance);
            if (state.Equals(previous))
                return;

            _property.Set(instance, state);

            _observer.OnNext(new StateChangedImpl(instance, previous, state));
        }


        class StateChangedImpl :
            StateChanged<TInstance>
        {
            public StateChangedImpl(TInstance instance, State<TInstance> previous, State<TInstance> current)
            {
                Instance = instance;
                Previous = previous;
                Current = current;
            }

            public TInstance Instance { get; private set; }

            public State<TInstance> Previous { get; private set; }

            public State<TInstance> Current { get; private set; }
        }
    }
}