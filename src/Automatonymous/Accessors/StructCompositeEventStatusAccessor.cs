namespace Automatonymous.Accessors
{
    using System.Reflection;
    using GreenPipes;
    using GreenPipes.Internals.Reflection;


    public class StructCompositeEventStatusAccessor<TInstance> : CompositeEventStatusAccessor<TInstance>
    {
        readonly ReadWriteProperty<TInstance, CompositeEventStatus> _property;

        public StructCompositeEventStatusAccessor(PropertyInfo propertyInfo)
        {
            _property = new ReadWriteProperty<TInstance, CompositeEventStatus>(propertyInfo);
        }

        public CompositeEventStatus Get(TInstance instance) =>
            _property.Get(instance);

        public void Set(TInstance instance, CompositeEventStatus status) =>
            _property.Set(instance, status);

        public void Probe(ProbeContext context) =>
            context.Add("property", _property.Property.Name);
    }
}
