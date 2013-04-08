// Copyright 2012-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, either express or implied. See the License for the specific language governing
// permissions and limitations under the License.
namespace TaskComposition
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Composers are passed to vanes, which use them to add continuations and/or compensations
    /// </summary>
    public interface Composer
    {
        /// <summary>
        /// The CancellationToken for this execution
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Adds a continuation to the plan
        /// </summary>
        /// <param name="continuation"></param>
        /// <param name="runSynchronously"></param>
        /// <returns></returns>
        Composer Execute(Action continuation, bool runSynchronously = true);

        /// <summary>
        /// Adds a task continuation to the plan. If the Task is not truly asynchronous, it can be
        /// invoked synchronously retaining maximum performance.
        /// </summary>
        /// <param name="continuationTask"></param>
        /// <param name="runSynchronously"></param>
        /// <returns></returns>
        Composer Execute(Func<Task> continuationTask, bool runSynchronously = true);

        /// <summary>
        /// Adds a compensating task to the plan, which will be invoked if an exception occurs
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        Composer Compensate(Func<Compensation, CompensationResult> compensation);

        /// <summary>
        /// Adds a continuation that is always run, regardless of a successful or exceptional condition
        /// </summary>
        /// <param name="continuation"></param>
        /// <param name="runSynchronously"></param>
        /// <returns></returns>
        Composer Finally(Action<TaskStatus> continuation, bool runSynchronously = true);

        /// <summary>
        /// Adds a delay to the execution
        /// </summary>
        /// <param name="dueTime">The delay period, in milliseconds</param>
        /// <returns></returns>
        Composer Delay(int dueTime);

        /// <summary>
        /// Adds a successful completion of the execution to the plan
        /// </summary>
        void Completed();

        /// <summary>
        /// Fails the plan, causing any compensations to be executed
        /// </summary>
        /// <param name="exception"></param>
        void Failed(Exception exception);

        /// <summary>
        /// Compose a completed task
        /// </summary>
        /// <returns></returns>
        Task ComposeCompleted();
    }
}