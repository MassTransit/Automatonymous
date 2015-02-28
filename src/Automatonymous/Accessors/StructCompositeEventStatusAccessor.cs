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
    using System.Reflection;
    using Internals;


    public class StructCompositeEventStatusAccessor<TInstance> :
        CompositeEventStatusAccessor<TInstance>
    {
        readonly ReadWriteProperty<TInstance, CompositeEventStatus> _property;

        public StructCompositeEventStatusAccessor(PropertyInfo propertyInfo)
        {
            _property = new ReadWriteProperty<TInstance, CompositeEventStatus>(propertyInfo);
        }

        public CompositeEventStatus Get(TInstance instance)
        {
            return _property.Get(instance);
        }

        public void Set(TInstance instance, CompositeEventStatus status)
        {
            _property.Set(instance, status);
        }
    }
}