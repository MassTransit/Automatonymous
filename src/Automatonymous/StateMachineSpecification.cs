namespace Stayt
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Impl;

    public abstract class StateMachineSpecification<TInstance>
        where TInstance : StateMachineInstance
    {
        protected StateMachineSpecification()
        {
            State(() => Initial);
            State(() => Completed);
        }

        public State Initial { get; private set; }
        public State Completed { get; private set; }

        protected void Event(Expression<Func<Event>> propertyExpression)
        {
            var property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var @event = new EventImpl<TInstance>(name);

            property.SetValue(this, @event, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);
        }

        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression)
        {
            var property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var @event = new EventImpl<TInstance, T>(name);

            property.SetValue(this, @event, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);
        }

        protected void State(Expression<Func<State>> propertyExpression)
        {
            var property = GetPropertyInfo(propertyExpression);

            string name = property.Name;

            var @event = new StateImpl<TInstance>(name);

            property.SetValue(this, @event, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                              null, null);

        }

        static PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Must be a member expression");

            if (memberExpression.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Must be a property expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Not a property, wtF?");

            return property;
        }

        protected void During(State state, params int[] args)
        {
        }

        protected void Initially(params int[] args)
        {
            //During(_states["Initial"], args);
        }

        protected int When(Event @event)
        {
            return 0;
        }

        protected int When<T>(Event<T> @event)
        {
            return 0;
        }
    }
}