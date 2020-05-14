namespace Automatonymous.Binders
{
    using System.Collections.Generic;


    public interface EventActivities<TInstance>
        where TInstance : class
    {
        IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders();
    }
}
