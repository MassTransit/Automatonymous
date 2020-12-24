namespace Automatonymous.Binders
{
    using System;
    using System.Collections.Generic;
    using Activities;


    public class CatchExceptionActivityBinder<TInstance, TException> :
        ExceptionActivityBinder<TInstance, TException>
        where TInstance : class
        where TException : Exception
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachine<TInstance> _machine;

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event)
        {
            _activities = new ActivityBinder<TInstance>[0];
            _machine = machine;
            _event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine => _machine;

        public Event Event => _event;

        public ExceptionActivityBinder<TInstance, TException> Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
            where T : Exception
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> If(
            StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TException> IfAsync(
            StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TException> IfElse(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TException>(_event, condition, thenBinder, elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, conditionBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> IfElseAsync(
            StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TException>(_event, condition, thenBinder, elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, _event, _activities, conditionBinder);
        }

        ExceptionActivityBinder<TInstance, TException> GetBinder(
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> callback)
        {
            ExceptionActivityBinder<TInstance, TException> thenBinder = new CatchExceptionActivityBinder<TInstance, TException>(_machine,
                _event);
            return callback(thenBinder);
        }
    }


    public class CatchExceptionActivityBinder<TInstance, TData, TException> :
        ExceptionActivityBinder<TInstance, TData, TException>
        where TInstance : class
        where TException : Exception
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachine<TInstance> _machine;

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _activities = new ActivityBinder<TInstance>[0];
            _machine = machine;
            _event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            _event = @event;
        }

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine => _machine;

        public Event<TData> Event => _event;

        public ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Add(Activity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, converterActivity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
            where T : Exception
        {
            ExceptionActivityBinder<TInstance, TData, T> binder = new CatchExceptionActivityBinder<TInstance, TData, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> If(
            StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfAsync(
            StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfElse(
            StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TData, TException>(_event, condition, thenBinder,
                elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, conditionBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfElseAsync(
            StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TData, TException>(_event, condition, thenBinder,
                elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event, _activities, conditionBinder);
        }

        ExceptionActivityBinder<TInstance, TData, TException> GetBinder(
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> callback)
        {
            ExceptionActivityBinder<TInstance, TData, TException> binder =
                new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, _event);
            return callback(binder);
        }
    }
}
