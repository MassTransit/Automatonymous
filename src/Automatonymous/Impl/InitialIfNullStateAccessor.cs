﻿// Copyright 2011 Chris Patterson, Dru Sellers
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
    using System.Linq.Expressions;
    using Activities;


    public class InitialIfNullStateAccessor<TInstance> :
        StateAccessor<TInstance>
        where TInstance : class
    {
        readonly IObserver<StateChanged<TInstance>> _observer;
        readonly TransitionActivity<TInstance> _initialActivity;
        readonly StateAccessor<TInstance> _rawStateAccessor;

        public InitialIfNullStateAccessor(Expression<Func<TInstance, State>> currentStateExpression,
                                          State<TInstance> initialState, IObserver<StateChanged<TInstance>> observer)
        {
            _observer = observer;
            _rawStateAccessor = new RawStateAccessor<TInstance>(currentStateExpression, _observer);

            _initialActivity = new TransitionActivity<TInstance>(initialState, _rawStateAccessor);
        }

        public State<TInstance> Get(TInstance instance)
        {
            State<TInstance> state = _rawStateAccessor.Get(instance);
            if (state == null)
            {
                _initialActivity.Execute(instance);
                state = _rawStateAccessor.Get(instance);
            }

            return state;
        }

        public void Set(TInstance instance, State<TInstance> state)
        {
            _rawStateAccessor.Set(instance, state);
        }
    }
}