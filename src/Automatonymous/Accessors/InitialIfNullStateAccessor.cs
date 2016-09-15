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
namespace Automatonymous.Accessors
{
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;
    using Contexts;
    using GreenPipes;


    public class InitialIfNullStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly Behavior<TInstance> _initialBehavior;
        readonly StateAccessor<TInstance> _stateAccessor;

        public InitialIfNullStateAccessor(State<TInstance> initialState, StateAccessor<TInstance> stateAccessor)
        {
            _stateAccessor = stateAccessor;

            Activity<TInstance> initialActivity = new TransitionActivity<TInstance>(initialState, _stateAccessor);
            _initialBehavior = new LastBehavior<TInstance>(initialActivity);
        }

        async Task<State<TInstance>> StateAccessor<TInstance>.Get(InstanceContext<TInstance> context)
        {
            State<TInstance> state = await _stateAccessor.Get(context).ConfigureAwait(false);
            if (state == null)
            {
                var behaviorContext = new EventBehaviorContext<TInstance>(context);

                await _initialBehavior.Execute(behaviorContext).ConfigureAwait(false);

                state = await _stateAccessor.Get(context).ConfigureAwait(false);
            }
            return state;
        }

        Task StateAccessor<TInstance>.Set(InstanceContext<TInstance> context, State<TInstance> state)
        {
            return _stateAccessor.Set(context, state);
        }

        public void Probe(ProbeContext context)
        {
            _stateAccessor.Probe(context);
        }
    }
}