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
    using System.Threading;
    using System.Threading.Tasks;
    using Impl;
    using Taskell;


    public static class EventLiftExtensions
    {
        public static EventLift<TInstance> CreateEventLift<TInstance>(
            this StateMachine<TInstance> stateMachine,
            Event @event)
            where TInstance : class
        {
            var eventLift = new EventLiftImpl<TInstance>(stateMachine, @event);

            return eventLift;
        }

        public static EventLift<TInstance, TData> CreateEventLift<TInstance, TData>(
            this StateMachine<TInstance> stateMachine,
            Event<TData> @event)
            where TInstance : class
        {
            var eventLift = new EventLiftImpl<TInstance, TData>(stateMachine, @event);

            return eventLift;
        }

        public static Task Raise<TInstance>(this EventLift<TInstance> lift, TInstance instance,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInstance : class
        {
            var composer = new TaskComposer<TInstance>(cancellationToken);

            lift.Raise(composer, instance);

            return composer.Finish();
        }

        public static Task Raise<TInstance, TData>(this EventLift<TInstance, TData> lift, TInstance instance, TData value,
            CancellationToken cancellationToken = default(CancellationToken))
            where TInstance : class
        {
            var composer = new TaskComposer<TInstance>(cancellationToken);

            lift.Raise(composer, instance, value);

            return composer.Finish();
        }
    }
}