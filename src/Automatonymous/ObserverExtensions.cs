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
namespace Automatonymous
{
    using System;


    public static class ObserverExtensions
    {
        public static IDisposable ConnectStateObserver<T>(this StateMachine<T> machine, StateObserver<T> observer) 
            where T : class
        {
            return machine.ConnectStateObserver(observer);
        }

        public static IDisposable ConnectEventObserver<T>(this StateMachine<T> machine, EventObserver<T> observer) 
            where T : class
        {
            return machine.ConnectEventObserver(observer);
        }

        public static IDisposable ConnectEventObserver<T>(this StateMachine<T> machine, Event @event, EventObserver<T> observer) 
            where T : class
        {
            return machine.ConnectEventObserver(@event, observer);
        }
    }
}