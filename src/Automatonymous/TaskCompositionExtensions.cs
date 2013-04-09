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
namespace Automatonymous
{
    using System.Threading.Tasks;
    using Taskell;


    public static class TaskCompositionExtensions
    {
        public static Task ComposeActivity<T>(this Composer composer, Activity<T> activity, T instance, bool runSynchronously = true)
        {
            var taskComposer = new TaskComposer<T>(composer.CancellationToken, runSynchronously);

            activity.Execute(composer, instance);

            return taskComposer.Finish();
        }

        public static Task ComposeActivity<T, TData>(this Composer composer, Activity<T> activity, T instance, TData data, bool runSynchronously = true)
        {
            var taskComposer = new TaskComposer<T>(composer.CancellationToken, runSynchronously);

            activity.Execute(composer, instance, data);

            return taskComposer.Finish();
        }

        public static Task ComposeActivity<T, TData>(this Composer composer, Activity<T, TData> activity, T instance, TData data,
            bool runSynchronously = true)
        {
            var taskComposer = new TaskComposer<T>(composer.CancellationToken, runSynchronously);

            activity.Execute(composer, instance, data);

            return taskComposer.Finish();
        }

        public static Task ComposeEvent<T>(this Composer composer, T instance, State<T> state, Event @event, bool runSynchronously = true)
        {
            var taskComposer = new TaskComposer<T>(composer.CancellationToken, runSynchronously);

            state.Raise(taskComposer, instance, @event);

            return taskComposer.Finish();
        }

        public static Task ComposeEvent<T, TData>(this Composer composer, T instance, State<T> state, Event<TData> @event, TData data,
            bool runSynchronously = true)
        {
            var taskComposer = new TaskComposer<T>(composer.CancellationToken, runSynchronously);

            state.Raise(taskComposer, instance, @event, data);

            return taskComposer.Finish();
        }
    }
}