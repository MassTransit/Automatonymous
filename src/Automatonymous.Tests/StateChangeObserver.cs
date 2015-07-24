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
namespace Automatonymous.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    class StateChangeObserver<T> :
        StateObserver<T>
        where T : class
    {
        public StateChangeObserver()
        {
            Events = new List<StateChange>();
        }

        public IList<StateChange> Events { get; private set; }

        public async Task StateChanged(InstanceContext<T> context, State currentState, State previousState)
        {
            Events.Add(new StateChange(context, currentState, previousState));
        }


        public struct StateChange
        {
            public InstanceContext<T> Context;
            public readonly State Current;
            public readonly State Previous;

            public StateChange(InstanceContext<T> context, State current, State previous)
            {
                Context = context;
                Current = current;
                Previous = previous;
            }
        }
    }
}