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
    using System.Collections.Generic;
    using Internal.Caching;


    public class StateImpl<TInstance> :
        State<TInstance>
        where TInstance : StateMachineInstance
    {
        readonly Cache<Event, List<Activity<TInstance>>> _activityCache;

        public StateImpl(string name)
        {
            Name = name;

            Enter = new SimpleEvent<TInstance>(name + ".Enter");
            Leave = new SimpleEvent<TInstance>(name + ".Leave");

            _activityCache = new DictionaryCache<Event, List<Activity<TInstance>>>(x => new List<Activity<TInstance>>());
        }

        public string Name { get; private set; }

        public Event Enter { get; private set; }
        public Event Leave { get; private set; }

        public void Inspect(StateMachineInspector inspector)
        {
            inspector.Inspect(this,
                              _ =>
                                  {
                                      _activityCache.Each(
                                                          (key, value) =>
                                                              {
                                                                  inspector.Inspect(key,
                                                                                    __ =>
                                                                                        {
                                                                                            value.ForEach(
                                                                                                          activity =>
                                                                                                              {
                                                                                                                  inspector
                                                                                                                      .
                                                                                                                      Inspect
                                                                                                                      (activity);
                                                                                                              });
                                                                                        });
                                                              });
                                  });
        }

        public void Raise(TInstance instance, Event @event, object value)
        {
            _activityCache.WithValue(@event,
                                     activities => { activities.ForEach(activity => { activity.Execute(instance); }); });
        }

        public void Bind(EventActivity<TInstance> activity)
        {
            _activityCache[activity.Event].Add(activity);
        }

        public override string ToString()
        {
            return string.Format("{0} (State)", Name);
        }
    }
}