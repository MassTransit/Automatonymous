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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;


    /// <summary>
    /// The default state accessor will attempt to find and use a single State property on the
    /// instance type. If no State property is found, or more than one is found, an exception
    /// will be thrown
    /// </summary>
    public class DefaultInstanceStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly Lazy<StateAccessor<TInstance>> _accessor;
        readonly State<TInstance> _initialState;
        readonly StateMachine<TInstance> _machine;
        readonly IObserver<StateChanged<TInstance>> _observer;

        public DefaultInstanceStateAccessor(StateMachine<TInstance> machine, State<TInstance> initialState,
            IObserver<StateChanged<TInstance>> observer)
        {
            _machine = machine;
            _initialState = initialState;
            _observer = observer;
            _accessor = new Lazy<StateAccessor<TInstance>>(CreateDefaultAccessor);
        }

        Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            return _accessor.Value.Get(context);
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            return _accessor.Value.Set(context, state);
        }

        StateAccessor<TInstance> CreateDefaultAccessor()
        {
            List<PropertyInfo> states = typeof(TInstance)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(State))
                .Where(x => x.GetGetMethod(true) != null)
                .Where(x => x.GetSetMethod(true) != null)
                .ToList();

            if (states.Count > 1)
            {
                throw new AutomatonymousException(
                    "The InstanceState was not configured, and could not be automatically identified as multiple State properties exist.");
            }

            if (states.Count == 0)
            {
                throw new AutomatonymousException(
                    "The InstanceState was not configured, and no public State property exists.");
            }

            ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
            MemberExpression memberExpression = Expression.Property(instance, states[0]);

            Expression<Func<TInstance, State>> expression = Expression.Lambda<Func<TInstance, State>>(memberExpression,
                instance);

            return new InitialIfNullStateAccessor<TInstance>(_machine, expression, _initialState, _observer);
        }
    }
}