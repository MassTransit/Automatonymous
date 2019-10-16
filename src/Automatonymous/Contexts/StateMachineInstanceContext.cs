// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Contexts
{
    using System.Threading;
    using GreenPipes;


    public class StateMachineInstanceContext<TInstance> :
        BasePipeContext,
        InstanceContext<TInstance>
        where TInstance : class
    {
        public StateMachineInstanceContext(TInstance instance)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, params object[] payloads)
            : base(payloads)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Instance = instance;
        }

        public StateMachineInstanceContext(TInstance instance, CancellationToken cancellationToken, params object[] payloads)
            : base(cancellationToken, payloads)
        {
            Instance = instance;
        }

        public TInstance Instance { get; }
    }
}
