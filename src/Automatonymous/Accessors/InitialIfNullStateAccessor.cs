// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Accessors
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;
    using Contexts;


    public class InitialIfNullStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly Behavior<TInstance> _initialBehavior;
        readonly StateAccessor<TInstance> _rawStateAccessor;

        public InitialIfNullStateAccessor(StateMachine<TInstance> machine, Expression<Func<TInstance, State>> currentStateExpression,
            State<TInstance> initialState, IObserver<StateChanged<TInstance>> observer)
        {
            _rawStateAccessor = new RawStateAccessor<TInstance>(machine, currentStateExpression, observer);

            Activity<TInstance> initialActivity = new TransitionActivity<TInstance>(initialState, _rawStateAccessor);
            _initialBehavior = new LastBehavior<TInstance>(initialActivity);
        }

        async Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            State<TInstance> state = await _rawStateAccessor.Get(context);
            if (state == null)
            {
                var behaviorContext = new EventBehaviorContext<TInstance>(context);

                await _initialBehavior.Execute(behaviorContext);

                state = await _rawStateAccessor.Get(context);
            }
            return state;
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            return _rawStateAccessor.Set(context, state);
        }
    }
}