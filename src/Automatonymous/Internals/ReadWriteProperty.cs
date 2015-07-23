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
namespace Automatonymous.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    class ReadWriteProperty<T, TProperty> :
        ReadOnlyProperty<T, TProperty>
    {
        public readonly Action<T, TProperty> SetProperty;

        public ReadWriteProperty(PropertyInfo property)
            : base(property)
        {
            SetProperty = GetSetMethod(Property);
        }

        public void Set(T instance, TProperty value)
        {
            SetProperty(instance, value);
        }

        static Action<T, TProperty> GetSetMethod(PropertyInfo property)
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(TProperty), "value");
            MethodCallExpression call = Expression.Call(instance, property.SetMethod, value);
            return Expression.Lambda<Action<T, TProperty>>(call, instance, value).Compile();
        }
    }
}