﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using System.Data;
    using Internals.Caching;
    using NHibernate;
    using NHibernate.SqlTypes;


    public class StringStateUserTypeConverter<T> :
        StateUserTypeConverter
        where T : StateMachine, new()
    {
// ReSharper disable StaticFieldInGenericType
        static readonly SqlType[] _types = new[] {NHibernateUtil.String.SqlType};
// ReSharper restore StaticFieldInGenericType
        readonly T _machine;
        readonly Cache<string, State> _stateCache;

        public StringStateUserTypeConverter()
        {
            _machine = new T();
            Cache<string, State> states = new DictionaryCache<string, State>();
            foreach (State state in _machine.States)
                states.Add(state.Name, state);

            _stateCache = states;
        }

        public SqlType[] Types
        {
            get { return _types; }
        }

        public State Get(IDataReader rs, string[] names)
        {
            var value = (string)NHibernateUtil.String.NullSafeGet(rs, names);

            State state = _stateCache[value];

            return state;
        }

        public void Set(IDbCommand command, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(command, null, index);
                return;
            }

            value = ((State)value).Name;

            NHibernateUtil.String.NullSafeSet(command, value, index);
        }
    }
}