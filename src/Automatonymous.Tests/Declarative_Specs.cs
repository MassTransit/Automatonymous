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
	using NUnit.Framework;


	[TestFixture]
	public class When_an_instance_has_multiple_states
	{
		[Test]
		public void Should_handle_both_states()
		{
			Assert.AreEqual(_top.Greeted, _instance.Top);
			Assert.AreEqual(_bottom.Ignored, _instance.Bottom);
		}

		MyState _instance;
		TopInstanceStateMachine _top;
		BottomInstanceStateMachine _bottom;

		[TestFixtureSetUp]
		public void Specifying_an_event_activity_with_data()
		{
			_instance = new MyState();

			_top = new TopInstanceStateMachine();
			_bottom = new BottomInstanceStateMachine();

			_top.RaiseEvent(_instance, _top.Initialized, new Init
			{
				Value = "Hello"
			});

			_bottom.RaiseEvent(_instance, _bottom.Initialized, new Init
			{
				Value = "Goodbye"
			});
		}

		class MyState
		{
			public State Top { get; set; }
			public State Bottom { get;  set; }
		}


		class Init
		{
			public string Value { get; set; }
		}


		class TopInstanceStateMachine :
			AutomatonymousStateMachine<MyState>
		{
			public TopInstanceStateMachine()
			{
				InstanceState(x => x.Top);

				State(() => Greeted);

				Event(() => Initialized);

				During(Initial,
					When(Initialized)
						.TransitionTo(Greeted));
			}

			public State Greeted { get; private set; }

			public Event<Init> Initialized { get; private set; }
		}
		
		class BottomInstanceStateMachine :
			AutomatonymousStateMachine<MyState>
		{
			public BottomInstanceStateMachine()
			{
				InstanceState(x => x.Bottom);

				State(() => Ignored);

				Event(() => Initialized);

				During(Initial,
					When(Initialized)
						.TransitionTo(Ignored));
			}

			public State Ignored { get; private set; }

			public Event<Init> Initialized { get; private set; }
		}
	}
}