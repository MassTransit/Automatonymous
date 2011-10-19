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
namespace Automatonymous.Util
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    public class FastProperty<T, TProperty>
    {
        public readonly Func<T, TProperty> GetDelegate;
        public readonly Action<T, TProperty> SetDelegate;

        public FastProperty(PropertyInfo property)
        {
            Property = property;
            GetDelegate = GetGetMethod(Property);
            SetDelegate = GetSetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public TProperty Get(T instance)
        {
            return GetDelegate(instance);
        }

        public void Set(T instance, TProperty value)
        {
            SetDelegate(instance, value);
        }

        static Action<T, TProperty> GetSetMethod(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(TProperty), "value");
            MethodCallExpression call = Expression.Call(instance, property.GetSetMethod(true), value);

            return Expression.Lambda<Action<T, TProperty>>(call, new[] {instance, value}).Compile();
        }

        static Func<T, TProperty> GetGetMethod(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            return
                Expression.Lambda<Func<T, TProperty>>(Expression.Call(instance, property.GetGetMethod()), instance).
                    Compile();
        }
    }
}