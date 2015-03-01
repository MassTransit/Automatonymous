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
namespace Automatonymous.Binders
{
    using Behaviors;


    public interface ActivityBinder<TInstance>
    {
        /// <summary>
        /// Returns True if the event is a state transition event (enter/leave/afterLeave/beforeEnter)
        /// for the specified state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool IsStateTransitionEvent(State state);

        /// <summary>
        /// Binds the activity to the state, may also just ignore the event if it's an ignore event
        /// </summary>
        /// <param name="state"></param>
        void Bind(State<TInstance> state);

        /// <summary>
        /// Bind the activities to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Bind(BehaviorBuilder<TInstance> builder);
    }
}