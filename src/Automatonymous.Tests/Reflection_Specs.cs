namespace Automatonymous.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
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

    [TestFixture]
    public class When_getting_static_properties
    {
        [Test]
        public void can_get_property_on_stand_alone_class()
        {
            var props = GetAllStaticProperties(typeof(SuperTarget));
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("StaticProp"));
        }

        [Test]
        public void can_get_single_property_on_super_from_sub()
        {
            var props = GetAllStaticProperties(typeof(SubTarget));
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("StaticProp"));
        }

        [Test]
        public void can_get_private_static_properties()
        {
            var props = GetAllStaticProperties(typeof(PrivateStatics));
            Assert.That(props.Count(), Is.EqualTo(2));
            var names = props.Select(x => x.Name);
            CollectionAssert.Contains(names, "CanWeGetPrivates");
            CollectionAssert.Contains(names, "StaticProp");
        }

        [Test]
        public void can_get_even_with_private_getter()
        {
            var props = GetAllStaticProperties(typeof(StaticsNoGetter));
            Assert.That(props.Count(), Is.EqualTo(2));
            var names = props.Select(x => x.Name);
            CollectionAssert.Contains(names, "ZupMan");
            CollectionAssert.Contains(names, "StaticProp");
        }

        [Test]
        public void can_get_with_no_hierarchy()
        {
            var props = GetAllStaticProperties(typeof(StaticsNoGetter), false);
            Assert.That(props.Count(), Is.EqualTo(1));
            Assert.That(props.First().Name, Is.EqualTo("ZupMan"));
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