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
namespace Automatonymous.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals.Caching;


    public class StateImpl<TInstance> :
        State<TInstance>
        where TInstance : class
    {
        readonly Cache<Event, List<Activity<TInstance>>> _activityCache;
        readonly string _name;
        readonly IObserver<EventRaised<TInstance>> _raisedObserver;
        readonly IObserver<EventRaising<TInstance>> _raisingObserver;

        public StateImpl(string name, IObserver<EventRaising<TInstance>> raisingObserver,
            IObserver<EventRaised<TInstance>> raisedObserver)
        {
            _name = name;
            _raisingObserver = raisingObserver;
            _raisedObserver = raisedObserver;

            Enter = new SimpleEvent(name + ".Enter");
            Leave = new SimpleEvent(name + ".Leave");

            BeforeEnter = new DataEvent<State>(name + ".BeforeEnter");
            AfterLeave = new DataEvent<State>(name + ".AfterLeave");

            _activityCache = new DictionaryCache<Event, List<Activity<TInstance>>>(x => new List<Activity<TInstance>>());
        }

        public string Name
        {
            get { return _name; }
        }

        public Event Enter { get; private set; }
        public Event Leave { get; private set; }

        public Event<State> BeforeEnter { get; private set; }
        public Event<State> AfterLeave { get; private set; }

        public void Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, _ => _activityCache.Each((key, value) =>
                {
                    key.Accept(inspector);
                    value.ForEach(activity => activity.Accept(inspector));
                }));
        }

        public void Raise(TInstance instance, Event @event)
        {
            List<Activity<TInstance>> activities;
            if (!_activityCache.TryGetValue(@event, out activities))
                return;

            var notification = new EventNotification(instance, @event);

            _raisingObserver.OnNext(notification);

            activities.ForEach(activity => activity.Execute(instance));

            _raisedObserver.OnNext(notification);
        }

        public Task<TInstance> RaiseAsync(TInstance instance, Event @event)
        {
            List<Activity<TInstance>> activities;
            if (!_activityCache.TryGetValue(@event, out activities))
                return CreateCompletedTask(instance);

            var notification = new EventNotification(instance, @event);

            Task<TInstance> task = Task<TInstance>.Factory.StartNew(() =>
            {
                _raisingObserver.OnNext(notification);
                return instance;
            });

            activities.ForEach(activity =>
            {
                var asyncActivity = activity as AsyncActivity<TInstance>;
                if (asyncActivity != null)
                    task = ChainTask(task, () => asyncActivity.ExecuteAsync(instance));
                else
                {
                    task = ChainTask(task, () => Task<TInstance>.Factory.StartNew(() =>
                    {
                        activity.Execute(instance);
                        return instance;
                    }));
                }
            });

            task = ChainTask(task, () => Task<TInstance>.Factory.StartNew(() =>
            {
                _raisedObserver.OnNext(notification);
                return instance;
            }));

            return task;
        }

        public void Raise<TData>(TInstance instance, Event<TData> @event, TData value)
        {
            List<Activity<TInstance>> activities;
            if (!_activityCache.TryGetValue(@event, out activities))
                return;

            var notification = new EventNotification(instance, @event);

            _raisingObserver.OnNext(notification);

            activities.ForEach(activity => activity.Execute(instance, value));

            _raisedObserver.OnNext(notification);
        }

        public Task<TInstance> RaiseAsync<TData>(TInstance instance, Event<TData> @event, TData value)
        {
            List<Activity<TInstance>> activities;
            if (!_activityCache.TryGetValue(@event, out activities))
                return CreateCompletedTask(instance);

            var notification = new EventNotification(instance, @event);

            Task<TInstance> task = Task<TInstance>.Factory.StartNew(() =>
                {
                    _raisingObserver.OnNext(notification);
                    return instance;
                });

            activities.ForEach(activity =>
                {
                    var asyncActivity = activity as AsyncActivity<TInstance>;
                    if (asyncActivity != null)
                        task = ChainTask(task, () => asyncActivity.ExecuteAsync(instance, value));
                    else
                    {
                        task = ChainTask(task, () => Task<TInstance>.Factory.StartNew(() =>
                            {
                                activity.Execute(instance, value);
                                return instance;
                            }));
                    }
                });

            task = ChainTask(task, () => Task<TInstance>.Factory.StartNew(() =>
                {
                    _raisedObserver.OnNext(notification);
                    return instance;
                }));

            return task;
        }


        public void Bind(EventActivity<TInstance> activity)
        {
            _activityCache[activity.Event].Add(activity);
        }

        public IEnumerable<Event> Events
        {
            get { return _activityCache.GetAllKeys(); }
        }

        public int CompareTo(State other)
        {
            return string.CompareOrdinal(_name, other.Name);
        }

        static Task<TInstance> CreateCompletedTask(TInstance instance)
        {
            var source = new TaskCompletionSource<TInstance>(TaskCreationOptions.None);
            source.SetResult(instance);
            Task<TInstance> task = source.Task;
            return task;
        }

        public override string ToString()
        {
            return string.Format("{0} (State)", _name);
        }

        static Task<TInstance> ChainTask(Task<TInstance> first, Func<Task<TInstance>> next)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (next == null)
                throw new ArgumentNullException("next");

            var source = new TaskCompletionSource<TInstance>();
            first.ContinueWith(antecedent =>
                {
                    if (first.IsFaulted)
                        source.TrySetException(first.Exception.InnerExceptions);
                    else if (first.IsCanceled)
                        source.TrySetCanceled();
                    else
                    {
                        try
                        {
                            Task<TInstance> t = next();
                            if (t == null)
                                source.TrySetCanceled();
                            else
                            {
                                t.ContinueWith(x =>
                                    {
                                        if (t.IsFaulted)
                                            source.TrySetException(t.Exception.InnerExceptions);
                                        else if (t.IsCanceled)
                                            source.TrySetCanceled();
                                        else
                                            source.TrySetResult(null);
                                    }, TaskContinuationOptions.ExecuteSynchronously);
                            }
                        }
                        catch (Exception ex)
                        {
                            source.TrySetException(ex);
                        }
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);
            return source.Task;
        }


        class EventNotification :
            EventRaising<TInstance>,
            EventRaised<TInstance>
        {
            public EventNotification(TInstance instance, Event @event)
            {
                Instance = instance;
                Event = @event;
            }

            public TInstance Instance { get; private set; }
            public Event Event { get; private set; }
        }
    }
}