namespace Automatonymous.NHibernateIntegration
{
    using System;
    using System.Linq.Expressions;
    using NHibernate.Mapping.ByCode;
    using NHibernate.Type;


    public static class AutomatonymousNHibernateExtensions
    {
        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper, Expression<Func<T, State>> stateExpression)
            where T : class
            where TMachine : StateMachine, new()
        {
            mapper.Property(stateExpression, x =>
            {
                x.Type<AutomatonymousStateUserType<TMachine>>();
                x.NotNullable(true);
                x.Length(80);
            });
        }
        
        public static void CombinedEventProperty<T, TMachine>(this IClassMapper<T> mapper, Expression<Func<T, Event>> combinedEventProperty)
            where T : class
            where TMachine : StateMachine, new()
        {
            mapper.Property(combinedEventProperty, x =>
                {
                    x.Type<Int32Type>();
                    x.NotNullable(true);
                });
        }
    }
}