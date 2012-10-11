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
namespace MassTransit.Testing
{
    using System.Collections.Generic;
    using Automatonymous;
    using Builders;
    using Saga;
    using Scenarios;
    using TestActions;


    public class StateMachineSagaTestBuilder<TScenario, TSaga, TStateMachine> :
        SagaTestBuilder<TScenario, TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : TestScenario
        where TStateMachine : StateMachine<TSaga>
    {
        readonly IList<TestAction<TScenario>> _actions;
        readonly TScenario _scenario;
        readonly TStateMachine _stateMachine;
        ISagaRepository<TSaga> _sagaRepository;

        public StateMachineSagaTestBuilder(TScenario scenario, TStateMachine stateMachine)
        {
            _scenario = scenario;
            _stateMachine = stateMachine;

            _actions = new List<TestAction<TScenario>>();
        }

        public SagaTest<TScenario, TSaga> Build()
        {
            if (_sagaRepository == null)
                _sagaRepository = new InMemorySagaRepository<TSaga>();

            var test = new StateMachineSagaTestInstance<TScenario, TSaga, TStateMachine>(_scenario, _actions,
                _sagaRepository, _stateMachine);

            return test;
        }

        public void SetSagaRepository(ISagaRepository<TSaga> sagaRepository)
        {
            _sagaRepository = sagaRepository;
        }

        public void AddTestAction(TestAction<TScenario> testAction)
        {
            _actions.Add(testAction);
        }
    }
}