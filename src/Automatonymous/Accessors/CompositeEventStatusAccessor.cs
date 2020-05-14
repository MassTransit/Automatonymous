namespace Automatonymous.Accessors
{
    using GreenPipes;


    public interface CompositeEventStatusAccessor<in TInstance> :
        IProbeSite
    {
        CompositeEventStatus Get(TInstance instance);

        void Set(TInstance instance, CompositeEventStatus status);
    }
}
