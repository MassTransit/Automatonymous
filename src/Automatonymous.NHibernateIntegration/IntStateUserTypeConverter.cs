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
namespace Automatonymous.NHibernateIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Internals.Caching;
    using NHibernate;
    using NHibernate.SqlTypes;

    /// <summary>
    /// Converts a State to an int, based on the given ordered array of states, for storage
    /// using NHibernate. If a new state is added, it must be added to the *end* of the array
    /// to avoid renumbering previously persisted state machine instances.
    /// </summary>
    /// <typeparam name="T">The state machine type</typeparam>
    public class IntStateUserTypeConverter<T> :
        StateUserTypeConverter
        where T : StateMachine
    {
// ReSharper disable StaticFieldInGenericType
        static readonly SqlType[] _types = new[] {NHibernateUtil.Int32.SqlType};
// ReSharper restore StaticFieldInGenericType
        readonly T _machine;
        readonly Cache<State, int> _stateToValueCache;
        readonly Cache<int, State> _valueToStateCache;

        public IntStateUserTypeConverter(T machine, params State[] states)
        {
            _machine = machine;
            if (_machine.States.Except(states).Any())
                throw new ArgumentOutOfRangeException("states", "One or more states are not specified");

            List<KeyValuePair<int, State>> allStates =
                states.Select((state, index) => new KeyValuePair<int, State>(index, state)).ToList();

            _valueToStateCache = new DictionaryCache<Int32, State>(allStates.ToDictionary(x => x.Key, x => x.Value));
            _stateToValueCache = new DictionaryCache<State, Int32>(allStates.ToDictionary(x => x.Value, x => x.Key));
        }

        public SqlType[] Types
        {
            get { return _types; }
        }

        public State Get(IDataReader rs, string[] names)
        {
            var value = (Int32)NHibernateUtil.Int32.NullSafeGet(rs, names);

            State state = _valueToStateCache[value];

            return state;
        }

        public void Set(IDbCommand command, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.Int32.NullSafeSet(command, null, index);
                return;
            }

            int setValue = _stateToValueCache[(State)value];

            NHibernateUtil.Int32.NullSafeSet(command, setValue, index);
        }
    }
}