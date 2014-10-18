// Copyright 2011 Chris Patterson, Dru Sellers
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Tests
{
	using System.Linq;

	using NUnit.Framework;


	[TestFixture]
	public class Using_a_simple_state_machine
	{
		[Test]
		public void Should_register_all_state_properties()
		{
			var stateMachine = new TestStateMachine();

			var states = stateMachine.States.ToList();

			Assert.That(states, Has.Count.EqualTo(4));
			Assert.That(states, Contains.Item(stateMachine.Initial));
			Assert.That(states, Contains.Item(stateMachine.Final));
			Assert.That(states, Contains.Item(stateMachine.ThisIsAState));
			Assert.That(states, Contains.Item(stateMachine.ThisIsAlsoAState));
		}

		[Test]
		public void Should_initialize_all_state_properties()
		{
			var stateMachine = new TestStateMachine();

			Assert.That(stateMachine.Initial, Is.Not.Null);
			Assert.That(stateMachine.Final, Is.Not.Null);
			Assert.That(stateMachine.ThisIsAState, Is.Not.Null);
			Assert.That(stateMachine.ThisIsAlsoAState, Is.Not.Null);
		}

		[Test]
		public void Should_register_all_event_properties()
		{
			var stateMachine = new TestStateMachine();

			var events = stateMachine.Events.ToList();

			Assert.That(events, Has.Count.EqualTo(2));
			Assert.That(events, Contains.Item(stateMachine.ThisIsASimpleEvent));
			Assert.That(events, Contains.Item(stateMachine.ThisIsAnEventConsumingData));
		}

		[Test]
		public void Should_initialize_all_event_properties()
		{
			var stateMachine = new TestStateMachine();

			Assert.That(stateMachine.ThisIsASimpleEvent, Is.Not.Null);
			Assert.That(stateMachine.ThisIsAnEventConsumingData, Is.Not.Null);
		}


		class Instance
		{
		}


		class EventData
		{
		}


		class TestStateMachine : AutomatonymousStateMachine<Instance>
		{
			public State ThisIsAState { get; private set; }
			public State ThisIsAlsoAState { get; private set; }
			public Event ThisIsASimpleEvent { get; private set; }
			public Event<EventData> ThisIsAnEventConsumingData { get; private set; }
		}
	}
}
