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
namespace Automatonymous.Activities
{
    using System.Threading.Tasks;


    /// <summary>
    /// Calls through a Behavior<typeparam name="TInstance">T</typeparam> retaining the data portion
    /// of the behavior context.
    /// </summary>
    /// <typeparam name="TInstance">The instance type</typeparam>
    /// <typeparam name="TData">The event data type</typeparam>
    public class SlimBehavior<TInstance, TData> :
        Behavior<TInstance, TData>
    {
        readonly Behavior<TInstance> _behavior;

        public SlimBehavior(Behavior<TInstance> behavior)
        {
            _behavior = behavior;
        }

        public Task Execute(BehaviorContext<TInstance, TData> context)
        {
            return _behavior.Execute(context);
        }

        void AcceptStateMachineInspector.Accept(StateMachineInspector inspector)
        {
            _behavior.Accept(inspector);
        }
    }
}