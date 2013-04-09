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
namespace Automatonymous
{
    using System;
    using Taskell;


    public interface InstanceLift<out T>
        where T : StateMachine
    {
        void Raise(Composer composer, Event @event);

        void Raise<TData>(Composer composer, Event<TData> @event, TData value);

        void Raise(Composer composer, Func<T, Event> eventSelector);

        void Raise<TData>(Composer composer, Func<T, Event<TData>> eventSelector, TData data);
    }
}