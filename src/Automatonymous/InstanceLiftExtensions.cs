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
    using TaskComposition;


    public static class InstanceLiftExtensions
    {
        public static InstanceLift<T> CreateInstanceLift<T, TInstance>(this T stateMachine, TInstance instance)
            where T : StateMachine<TInstance>
            where TInstance : class
        {
            var instanceLift = new InstanceLiftImpl<T, TInstance>(stateMachine, instance);

            return instanceLift;
        }


        public static Task Raise<T>(this InstanceLift<T> lift, Event @event,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine
        {
            var composer = new TaskComposer<T>(cancellationToken);

            lift.Raise(composer, @event);

            return composer.Finish();
        }

        public static Task Raise<T, TData>(this InstanceLift<T> lift, Event<TData> @event, TData data,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : StateMachine
        {
            var composer = new TaskComposer<T>(cancellationToken);

            lift.Raise(composer, @event, data);

            return composer.Finish();
        }
    }
}