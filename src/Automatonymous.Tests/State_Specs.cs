namespace Automatonymous.Tests
{
    using Impl;
    using NUnit.Framework;

    [TestFixture]
    public class When_a_state_is_declared
    {
        [Test]
        public void It_should_capture_the_name_of_final()
        {
            Assert.AreEqual("Final", _machine.Final.Name);
        }

        [Test]
        public void It_should_capture_the_name_of_initial()
        {
            Assert.AreEqual("Initial", _machine.Initial.Name);
        }

        [Test]
        public void It_should_capture_the_name_of_running()
        {
            Assert.AreEqual("Running", _machine.Running.Name);
        }

        [Test]
        public void Should_be_an_instance_of_the_proper_type()
        {
			Assert.IsInstanceOf<StateImpl<Instance>>(_machine.Initial);
        }

		class Instance
		{
			public State CurrentState { get; set; }
		}

        TestStateMachine _machine;

        [TestFixtureSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }

        class TestStateMachine :
			AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
			{
				InstanceStatePropertyAccessor(x => x.CurrentState);

                State(() => Running);
            }

            public State Running { get; private set; }
        }
    }
}