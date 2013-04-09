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
namespace Automatonymous.Activities
{
    using Taskell;


    public class TransitionActivity<TInstance> :
        Activity<TInstance>
        where TInstance : class
    {
        readonly StateAccessor<TInstance> _currentStateAccessor;
        readonly State<TInstance> _toState;

        public TransitionActivity(State<TInstance> toState, StateAccessor<TInstance> currentStateAccessor)
        {
            _toState = toState;
            _currentStateAccessor = currentStateAccessor;
        }

        public State ToState
        {
            get { return _toState; }
        }

        void AcceptStateMachineInspector.Accept(StateMachineInspector inspector)
        {
            inspector.Inspect(this, x => { });
        }

        void Activity<TInstance>.Execute(Composer composer, TInstance instance)
        {
            Transition(composer, instance);
        }

        void Activity<TInstance>.Execute<T>(Composer composer, TInstance instance, T value)
        {
            Transition(composer, instance);
        }

        void Transition(Composer composer, TInstance instance)
        {
            composer.Execute(() =>
                {
                    State<TInstance> currentState = _currentStateAccessor.Get(instance);
                    if (currentState == _toState)
                        return composer.ComposeCompleted();

                    var taskComposer = new TaskComposer<TInstance>(composer.CancellationToken);

                    if (currentState != null)
                        currentState.Raise(taskComposer, instance, currentState.Leave);

                    _toState.Raise(taskComposer, instance, _toState.BeforeEnter, currentState);

                    ((Composer)taskComposer).Execute(() => _currentStateAccessor.Set(instance, _toState));

                    if (currentState != null)
                        currentState.Raise(taskComposer, instance, currentState.AfterLeave, _toState);

                    _toState.Raise(taskComposer, instance, _toState.Enter);

                    return taskComposer.Finish();
                });
        }
    }
}