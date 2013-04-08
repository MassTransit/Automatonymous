// Copyright 2012-2013 Chris Patterson
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
    /// Builds a chain of tasks that should run synchronously on the building thread until
    /// an asynchronous operation is requested, in which case it switches the chain to 
    /// asynchronous.
    /// </summary>
    /// <typeparam name="T">The payload type of the task chain</typeparam>
    public class TaskComposer<T> :
        Composer
    {
        readonly CancellationToken _cancellationToken;

        readonly Lazy<Exception> _completeException =
            new Lazy<Exception>(() => new TaskComposerException("The composition is already complete."));

        bool _composeFinished;
        Task _task;

        public TaskComposer(CancellationToken cancellationToken, bool runSynchronously = true)
        {
            _cancellationToken = cancellationToken;
            _task = runSynchronously
                        ? cancellationToken.IsCancellationRequested
                              ? TaskUtil.Cancelled()
                              : TaskUtil.Completed()
                        : Task.Factory.StartNew(() => { }, cancellationToken);
        }

        CancellationToken Composer.CancellationToken
        {
            get { return _cancellationToken; }
        }

        Composer Composer.Execute(Action continuation, bool runSynchronously)
        {
            if (_composeFinished)
                throw _completeException.Value;

            _task = Execute(_task, () => TaskUtil.RunSynchronously(continuation, _cancellationToken), _cancellationToken,
                runSynchronously);
            return this;
        }

        Composer Composer.Execute(Func<Task> continuationTask, bool runSynchronously)
        {
            if (_composeFinished)
                throw _completeException.Value;

            _task = Execute(_task, continuationTask, _cancellationToken, runSynchronously);
            return this;
        }

        Composer Composer.Compensate(Func<Compensation, CompensationResult> compensation)
        {
            if (_composeFinished)
                throw _completeException.Value;

            if (_task.Status == TaskStatus.RanToCompletion)
                return this;

            _task = Compensate(_task, x => compensation(new TaskCompensation<T>(x)).Task);
            return this;
        }

        Composer Composer.Finally(Action<TaskStatus> continuation, bool runSynchronously)
        {
            if (_composeFinished)
                throw _completeException.Value;

            if (_task.IsCompleted)
            {
                try
                {
                    continuation(_task.Status);
                    return this;
                }
                catch (Exception ex)
                {
                    _task.MarkObserved();
                    _task = TaskUtil.CompletedError(ex);
                    return this;
                }
            }

            _task = FinallyAsync(_task, continuation, runSynchronously);
            return this;
        }

        Composer Composer.Delay(int dueTime)
        {
            if (_composeFinished)
                throw _completeException.Value;

            if (dueTime < -1)
                throw new ArgumentOutOfRangeException("dueTime",
                    "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.");

            _task = Execute(_task, () => CreateDelayTask(dueTime, _cancellationToken), _cancellationToken);
            return this;
        }

        void Composer.Completed()
        {
            if (_composeFinished)
                throw _completeException.Value;

            _task = Execute(_task, TaskUtil.Completed, _cancellationToken);
        }

        void Composer.Failed(Exception exception)
        {
            if (_composeFinished)
                throw _completeException.Value;

            _task = Execute(_task, () => TaskUtil.CompletedError(exception), _cancellationToken);
        }

        public Task ComposeCompleted()
        {
            var composer = new TaskComposer<T>(_cancellationToken);
            return composer.Finish();
        }

        public Task Finish()
        {
            _composeFinished = true;

            return _task;
        }

        static Task Execute(Task task, Func<Task> continuationTask, CancellationToken cancellationToken,
            bool runSynchronously = true)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                    return TaskUtil.CompletedErrors(task.Exception.InnerExceptions);

                if (task.IsCanceled || cancellationToken.IsCancellationRequested)
                    return TaskUtil.Cancelled();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        return continuationTask();
                    }
                    catch (Exception ex)
                    {
                        return TaskUtil.CompletedError(ex);
                    }
                }
            }

            return ExecuteAsync(task, continuationTask, cancellationToken, runSynchronously);
        }

        static Task ExecuteAsync(Task task, Func<Task> continuationTask, CancellationToken cancellationToken,
            bool runSynchronously)
        {
            var source = new TaskCompletionSource<Task>();
            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsFaulted)
                        source.TrySetException(innerTask.Exception.InnerExceptions);
                    else if (innerTask.IsCanceled || cancellationToken.IsCancellationRequested)
                        source.TrySetCanceled();
                    else
                        source.TrySetResult(continuationTask());
                }, runSynchronously
                       ? TaskContinuationOptions.ExecuteSynchronously
                       : TaskContinuationOptions.None);

            return source.Task.FastUnwrap();
        }

        static Task CreateDelayTask(int dueTime, CancellationToken cancellationToken)
        {
            if (dueTime == 0)
                return TaskUtil.Completed();

            var source = new TaskCompletionSource<bool>();
            var registration = new CancellationTokenRegistration();
            var timer = new Timer(self =>
                {
                    registration.Dispose();
                    ((Timer)self).Dispose();
                    source.TrySetResult(true);
                });

            if (cancellationToken.CanBeCanceled)
            {
                Action callback = delegate
                    {
                        timer.Dispose();
                        source.TrySetCanceled();
                    };
                registration = cancellationToken.Register(callback);
            }

            timer.Change(dueTime, Timeout.Infinite);
            return source.Task;
        }

        static Task Compensate(Task task, Func<Task, Task> compensationTask)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    try
                    {
                        Task resultTask = compensationTask(task);
                        if (resultTask == null)
                            throw new InvalidOperationException("Sure could use a Task here buddy");

                        task.MarkObserved();

                        return resultTask;
                    }
                    catch (Exception ex)
                    {
                        return TaskUtil.CompletedError(ex);
                    }
                }

                if (task.IsCanceled)
                    return TaskUtil.Cancelled();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var tcs = new TaskCompletionSource<object>();
                    tcs.TrySetFromTask(task);
                    return tcs.Task;
                }
            }

            return CompensateAsync(task, compensationTask);
        }

        static Task CompensateAsync(Task task, Func<Task, Task> compensationTask)
        {
            var source = new TaskCompletionSource<Task>();

            task.ContinueWith(innerTask =>
                {
                    if (innerTask.IsCanceled)
                        source.TrySetCanceled();

                    return source.TrySetResult(TaskUtil.Completed());
                },
                TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            task.ContinueWith(innerTask =>
                {
                    try
                    {
                        Task resultTask = compensationTask(innerTask);
                        if (resultTask == null)
                            throw new InvalidOperationException("Sure could use a Task here buddy");

                        innerTask.MarkObserved();

                        source.TrySetResult(resultTask);
                    }
                    catch (Exception ex)
                    {
                        source.TrySetException(ex);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

            return source.Task.FastUnwrap();
        }


        static Task FinallyAsync(Task task, Action<TaskStatus> continuation, bool runSynchronously = true)
        {
            var source = new TaskCompletionSource<object>();
            task.ContinueWith(innerTask =>
                {
                    try
                    {
                        continuation(innerTask.Status);
                        source.TrySetFromTask(innerTask);
                    }
                    catch (Exception ex)
                    {
                        innerTask.MarkObserved();
                        source.TrySetException(ex);
                    }
                }, runSynchronously
                       ? TaskContinuationOptions.ExecuteSynchronously
                       : TaskContinuationOptions.None);

            return source.Task;
        }
    }
}