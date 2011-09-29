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
namespace Automatonymous.Internal
{
    using System;
    using System.Threading;
    using Caching;


    class Observable<T> :
        IObservable<T>,
        IObserver<T>
    {
        readonly Cache<int, IObserver<T>> _observers;

        int _key;

        public Observable()
        {
            _observers = new ConcurrentCache<int, IObserver<T>>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            int observerId = Interlocked.Increment(ref _key);
            
            _observers.Add(observerId, observer);

            return new ObserverReference(observerId, id => _observers.Remove(id));
        }

        public void OnNext(T value)
        {
            _observers.Each(x => x.OnNext(value));
        }

        public void OnError(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            _observers.Each(x => x.OnError(exception));
        }

        public void OnCompleted()
        {
            _observers.Each(x => x.OnCompleted());
        }


        class ObserverReference :
            IDisposable
        {
            readonly int _observerId;
            readonly Action<int> _removeObserver;

            public ObserverReference(int observerId, Action<int> removeObserver)
            {
                _observerId = observerId;
                _removeObserver = removeObserver;
            }

            public void Dispose()
            {
                _removeObserver(_observerId);
            }
        }
    }
}