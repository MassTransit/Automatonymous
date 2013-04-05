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
namespace Automatonymous.Tests
{
    using System;
    using System.Collections.Generic;


    class StateChangeObserver<T> :
        IObserver<StateChanged<T>>
        where T : class
    {
        public StateChangeObserver()
        {
            Events = new List<StateChanged<T>>();
        }

        public IList<StateChanged<T>> Events { get; private set; }
        public bool Completed { get; private set; }

        public void OnNext(StateChanged<T> value)
        {
            Events.Add(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
            Completed = true;
        }
    }
}