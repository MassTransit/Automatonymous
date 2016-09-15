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
namespace Automatonymous.Accessors
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using GreenPipes.Util;


    /// <summary>
    /// Accesses the current state as a string property
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    public class StringStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateMachine<TInstance> _machine;
        readonly StateObserver<TInstance> _observer;
        readonly ReadWriteProperty<TInstance, string> _property;

        public StringStateAccessor(StateMachine<TInstance> machine, Expression<Func<TInstance, string>> currentStateExpression,
            StateObserver<TInstance> observer)
        {
            _machine = machine;
            _observer = observer;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            string stateName = _property.Get(context.Instance);
            if (string.IsNullOrWhiteSpace(stateName))
                return Task.FromResult<State<TInstance>>(null);

            return Task.FromResult(_machine.GetState(stateName));
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            string previous = _property.Get(context.Instance);
            if (state.Name.Equals(previous))
                return TaskUtil.Completed;

            _property.Set(context.Instance, state.Name);

            State<TInstance> previousState = null;
            if (previous != null)
                previousState = _machine.GetState(previous);

            return _observer.StateChanged(context, state, previousState);
        }

        static ReadWriteProperty<TInstance, string> GetCurrentStateProperty(Expression<Func<TInstance, string>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, string>(propertyInfo);
        }
    }
}