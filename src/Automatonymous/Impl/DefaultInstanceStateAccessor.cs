// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;


    /// <summary>
    /// The default state accessor will attempt to find and use a single State property on the
    /// instance type. If no State property is found, or more than one is found, an exception
    /// will be thrown
    /// </summary>
    public class DefaultInstanceStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly State<TInstance> _initialState;
        readonly IObserver<StateChanged<TInstance>> _observer;

        StateAccessor<TInstance> _accessor;

        public DefaultInstanceStateAccessor(State<TInstance> initialState, IObserver<StateChanged<TInstance>> observer)
        {
            _initialState = initialState;
            _observer = observer;
        }

        StateAccessor<TInstance> Accessor
        {
            get { return _accessor ?? (_accessor = CreateDefaultAccessor()); }
        }

        State<TInstance> StateAccessor<TInstance>.Get(TInstance instance)
        {
            return Accessor.Get(instance);
        }

        void StateAccessor<TInstance>.Set(TInstance instance, State<TInstance> state)
        {
            Accessor.Set(instance, state);
        }

        StateAccessor<TInstance> CreateDefaultAccessor()
        {
            List<PropertyInfo> states = typeof(TInstance)
                .GetTypeInfo()
                .GetAllProperties()
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

            return new InitialIfNullStateAccessor<TInstance>(expression, _initialState, _observer);
        }
    }
}