// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A behavior is invoked by a state when an event is raised on the instance and embodies
    /// the activities that are executed in response to the event.
    /// </summary>
    public interface Behavior
    {
    }


    public interface Behavior<TInstance> :
        Behavior
    {
        Task Execute(BehaviorContext<TInstance> context, NextBehavior<TInstance> next);
        
        Task Execute<T>(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next);
    }

    public interface Behavior<TInstance, T> :
        Behavior
    {
        Task Execute(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next);
    }


    public interface NextBehavior<in TInstance, in T>
    {
        Task Execute(BehaviorContext<TInstance, T> context);
    }

    public interface NextBehavior<in TInstance>
    {
        Task Execute(BehaviorContext<TInstance> context);
    }


    public interface EventContext<out T> :
        EventContext
    {
        new Event<T> Event { get; }

        /// <summary>
        /// The data from the event
        /// </summary>
        T Data { get; }
    }

    // this needs an agenda to resolve so that the activities in a behavior
    // are executed in order, including any that are added to the agenda
    // think TaskFiber but using a tighter algorithm to avoid the noise
    // maybe a BlockingQueue? no that's not it since it's writing to itself...



    public interface EventContext
    {
        CancellationToken CancellationToken { get; }

        Event Event { get; }
    }


    public interface BehaviorContext<out TInstance, out T> :
        BehaviorContext<TInstance>,
        EventContext<T>
    {
        Behavior CurrentBehavior { get; }
    }

    public interface BehaviorContext<out TInstance> :
        EventContext
    {
        TInstance Instance { get; }
    }


    public interface BehaviorActivity<TInstance, T>
    {
        Task Execute(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next);
    }


    class ExecuteBehaviorActivity<TInstance, T> :
        BehaviorActivity<TInstance, T>
    {
        readonly Action<TInstance, T> _action;

        public ExecuteBehaviorActivity(Action<TInstance, T> action)
        {
            _action = action;
        }

        public async Task Execute(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next)
        {
            _action(context.Instance, context.Data);
        }
    }

    class TryCatchBehaviorActivity<TInstance, T> :
        BehaviorActivity<TInstance, T>
    {
        NextBehavior<TInstance, T> _exceptionHandler;

        public TryCatchBehaviorActivity(NextBehavior<TInstance, T> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public async Task Execute(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next)
        {
            Exception exception = null;
            try
            {

            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                var exceptionContext = context.Push(exception);

                await _exceptionHandler.Execute(exceptionContext);
            }
        }
    }


    class ActivityBehaviorActivity<TInstance, T> : 
        BehaviorActivity<TInstance, T>
    {
        Activity<TInstance, T> _activity;

        public ActivityBehaviorActivity(Activity<TInstance, T> activity)
        {
            _activity = activity;
        }

        public async Task Execute(BehaviorContext<TInstance, T> context, NextBehavior<TInstance, T> next)
        {
            await _activity.Execute(context.Instance, context.Data, context.CancellationToken);
       }
    }


    public interface Activity :
        AcceptStateMachineInspector
    {
    }


    public interface Activity<in TInstance> :
        Activity
    {
        Task Execute(TInstance instance, CancellationToken cancellationToken);

        Task Execute<T>(TInstance instance, T value, CancellationToken cancellationToken);
    }


    public interface Activity<in TInstance, in TData> :
        Activity
    {
        Task Execute(TInstance instance, TData value, CancellationToken cancellationToken);
    }
}