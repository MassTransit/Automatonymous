namespace Automatonymous.Binders
{
    using System;


    public interface ExceptionActivityBinder<TInstance, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event Event { get; }

        ExceptionActivityBinder<TInstance, TException> Add(Activity<TInstance> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> If(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> IfAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> IfElse(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback);
    }


    public interface ExceptionActivityBinder<TInstance, TData, TException> :
        EventActivities<TInstance>
        where TInstance : class
        where TException : Exception
    {
        StateMachine<TInstance> StateMachine { get; }

        Event<TData> Event { get; }

        ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance> activity);

        ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance, TData> activity);

        /// <summary>
        /// Catch an exception and execute the compensating activities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
            where T : Exception;

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> If(StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="activityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> IfAsync(
            StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> IfElse(StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback);

        /// <summary>
        /// Create a conditional branch of activities for processing
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenActivityCallback"></param>
        /// <param name="elseActivityCallback"></param>
        /// <returns></returns>
        ExceptionActivityBinder<TInstance, TData, TException> IfElseAsync(
            StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback);
    }
}
