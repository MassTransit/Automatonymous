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


    /// <summary>
    /// Accesses the current state as a string property
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    public class IntStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly StateAccessorIndex<TInstance> _index;
        readonly StateObserver<TInstance> _observer;
        readonly ReadWriteProperty<TInstance, int> _property;

        public IntStateAccessor(Expression<Func<TInstance, int>> currentStateExpression, StateAccessorIndex<TInstance> index,
            StateObserver<TInstance> observer)
        {
            _observer = observer;
            _index = index;

            _property = GetCurrentStateProperty(currentStateExpression);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            int stateIndex = _property.Get(context.Instance);

            return Task.FromResult(_index[stateIndex]);
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            int stateIndex = _index[state.Name];

            int previousIndex = _property.Get(context.Instance);

            if (stateIndex == previousIndex)
                return TaskUtil.Completed;

            _property.Set(context.Instance, stateIndex);

            State<TInstance> previousState = _index[previousIndex];

            return _observer.StateChanged(context, state, previousState);
        }

        static ReadWriteProperty<TInstance, int> GetCurrentStateProperty(Expression<Func<TInstance, int>> currentStateExpression)
        {
            PropertyInfo propertyInfo = currentStateExpression.GetPropertyInfo();

            return new ReadWriteProperty<TInstance, int>(propertyInfo);
        }
    }
}