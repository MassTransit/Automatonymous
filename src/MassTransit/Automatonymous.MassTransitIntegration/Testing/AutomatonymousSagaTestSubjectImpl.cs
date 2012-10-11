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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Automatonymous;
    using Saga;
    using Scenarios;
    using Subjects;
    using TestDecorators;


    public class AutomatonymousSagaTestSubjectImpl<TScenario, TSaga, TStateMachine> :
        SagaTestSubject<TSaga>
        where TSaga : class, SagaStateMachineInstance
        where TScenario : TestScenario
        where TStateMachine : StateMachine<TSaga>
    {
        readonly SagaListImpl<TSaga> _created;
        readonly ReceivedMessageListImpl _received;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly SagaListImpl<TSaga> _sagas;
        readonly TStateMachine _stateMachine;
        bool _disposed;
        UnsubscribeAction _unsubscribe;

        public AutomatonymousSagaTestSubjectImpl(ISagaRepository<TSaga> sagaRepository, TStateMachine stateMachine)
        {
            _sagaRepository = sagaRepository;
            _stateMachine = stateMachine;

            _received = new ReceivedMessageListImpl();
            _created = new SagaListImpl<TSaga>();
            _sagas = new SagaListImpl<TSaga>();
        }

        public ReceivedMessageList Received
        {
            get { return _received; }
        }

        public SagaList<TSaga> Created
        {
            get { return _created; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<SagaInstance<TSaga>> GetEnumerator()
        {
            return _sagas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Any()
        {
            return _sagas.Any();
        }

        public bool Any(Func<TSaga, bool> filter)
        {
            return _sagas.Any(filter);
        }

        public TSaga Contains(Guid sagaId)
        {
            return _sagas.Contains(sagaId);
        }

        public void Prepare(TScenario scenario)
        {
            var decoratedSagaRepository = new SagaRepositoryTestDecorator<TSaga>(_sagaRepository, _received, _created,
                _sagas);

            _unsubscribe = scenario.InputBus.SubscribeStateMachineSaga(_stateMachine, decoratedSagaRepository);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_unsubscribe != null)
                {
                    _unsubscribe();
                    _unsubscribe = null;
                }

                _received.Dispose();
            }

            _disposed = true;
        }

        ~AutomatonymousSagaTestSubjectImpl()
        {
            Dispose(false);
        }
    }
}