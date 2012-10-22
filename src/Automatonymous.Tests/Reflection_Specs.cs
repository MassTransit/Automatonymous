namespace Automatonymous.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using System.Reflection;

    class SuperTarget
    {
        public static string StaticProp { get; set; }
        public string InstanceProp { get; set; }
    }

    class SubTarget
        : SuperTarget
    {
        public string AnotherProp { get; private set; }
    }

    class PrivateStatics
        : SuperTarget
    {
        static string CanWeGetPrivates { get; set; }
    }

    class StaticsNoGetter
        : SuperTarget
    {
        public static string ZupMan { set; private get; }
    }

    
    public class When_getting_static_properties
    {
        [Fact]
        public void can_get_property_on_stand_alone_class()
        {
            var props = GetAllStaticProperties(typeof(SuperTarget));
            Assert.Equal(1, props.Count());
            Assert.Equal("StaticProp", props.First().Name);
        }

        [Fact]
        public void can_get_single_property_on_super_from_sub()
        {
            var props = GetAllStaticProperties(typeof(SubTarget));
            Assert.Equal(1, props.Count());
            Assert.Equal("StaticProp", props.First().Name);
        }

        [Fact]
        public void can_get_private_static_properties()
        {
            var props = GetAllStaticProperties(typeof(PrivateStatics));
            Assert.Equal(2, props.Count());
            var names = props.Select(x => x.Name);
            Assert.Contains("CanWeGetPrivates", names);
            Assert.Contains("StaticProp", names);
        }

        [Fact]
        public void can_get_even_with_private_getter()
        {
            var props = GetAllStaticProperties(typeof(StaticsNoGetter));
            Assert.Equal(2, props.Count());
            var names = props.Select(x => x.Name);
            Assert.Contains("ZupMan", names);
            Assert.Contains("StaticProp", names);
        }

        [Fact]
        public void can_get_with_no_hierarchy()
        {
            var props = GetAllStaticProperties(typeof(StaticsNoGetter), false);
            Assert.Equal(1, props.Count());
            Assert.Equal("ZupMan", props.First().Name);
        }

        static IEnumerable<PropertyInfo> GetAllStaticProperties(Type type, 
            bool flattenHierachy = true)
        {
            var info = type.GetTypeInfo();

            if (info.BaseType != null && flattenHierachy)
                foreach (var prop in GetAllStaticProperties(info.BaseType, true))
                    yield return prop;

            var props = info.DeclaredMethods
                            .Where(x => x.IsSpecialName && x.Name.StartsWith("get_") && x.IsStatic)
                            .Select(x => info.GetDeclaredProperty(x.Name.Substring("get_".Length)));

            foreach (var propertyInfo in props)
                yield return propertyInfo;
        }
    }
}