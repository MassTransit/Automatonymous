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
namespace Automatonymous.Behaviors
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Splits apart the data from the behavior so it can be invoked properly.
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public class DataBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        readonly Behavior<TInstance> _behavior;

        public DataBehavior(Behavior<TInstance> behavior)
        {
            _behavior = behavior;
        }

        void Visitable.Accept(StateMachineVisitor visitor)
        {
            _behavior.Accept(visitor);
        }

        public void Probe(ProbeContext context)
        {
            _behavior.Probe(context);
        }

        Task Behavior<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context)
        {
            return _behavior.Execute(context);
        }

        Task Behavior<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        {
            return _behavior.Faulted(context);
        }
    }
}