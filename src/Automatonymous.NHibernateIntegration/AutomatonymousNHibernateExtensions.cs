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
namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using NHibernate.Mapping.ByCode;
    using UserTypes;


    public static class AutomatonymousNHibernateExtensions
    {
        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression)
            where T : class
            where TMachine : StateMachine, new()
        {
            AutomatonymousStateUserType<TMachine>.SaveAsString(new TMachine());

            mapper.Property(stateExpression, x =>
                {
                    x.Type<AutomatonymousStateUserType<TMachine>>();
                    x.NotNullable(true);
                    x.Length(80);
                });
        }

        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression, TMachine machine)
            where T : class
            where TMachine : StateMachine, new()
        {
            AutomatonymousStateUserType<TMachine>.SaveAsString(machine);

            mapper.Property(stateExpression, x =>
                {
                    x.Type<AutomatonymousStateUserType<TMachine>>();
                    x.NotNullable(true);
                    x.Length(80);
                });
        }

        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression, TMachine machine, State[] statesInOrder)
            where T : class
            where TMachine : StateMachine, new()
        {
            AutomatonymousStateUserType<TMachine>.SaveAsInt32(machine, statesInOrder);

            mapper.Property(stateExpression, x =>
                {
                    x.Type<AutomatonymousStateUserType<TMachine>>();
                    x.NotNullable(true);
                    x.Length(80);
                });
        }

        public static void CompositeEventProperty<T>(this IClassMapper<T> mapper,
            Expression<Func<T, CompositeEventStatus>> compositeEventStatusExpression)
            where T : class
        {
            mapper.Property(compositeEventStatusExpression, x =>
                {
                    x.Type<CompositeEventStatusUserType>();
                    x.NotNullable(true);
                });
        }
    }
}