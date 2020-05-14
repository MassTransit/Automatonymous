namespace Automatonymous.Accessors
{
    using System.Reflection;
    using GreenPipes;
    using GreenPipes.Internals.Reflection;


    public class IntCompositeEventStatusAccessor<TInstance> :
        CompositeEventStatusAccessor<TInstance>
    {
        readonly ReadWriteProperty<TInstance, int> _property;

        public IntCompositeEventStatusAccessor(PropertyInfo propertyInfo)
        {
            _property = new ReadWriteProperty<TInstance, int>(propertyInfo);
        }

        public CompositeEventStatus Get(TInstance instance)
        {
            return new CompositeEventStatus(_property.Get(instance));
        }

        public void Set(TInstance instance, CompositeEventStatus status)
        {
            _property.Set(instance, status.Bits);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("property", _property.Property.Name);
        }
    }
}
