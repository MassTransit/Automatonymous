// Copyright 2011-2015 Chris Patterson, Dru Sellers
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
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Declaring_groups_in_a_state_machine
    {
        [Test]
        public void Should_allow_parallel_execution_of_events()
        {
        }

        [Test]
        public void Should_have_captured_initial_data()
        {
            Assert.AreEqual("Audi", _instance.VehicleMake);
            Assert.AreEqual("A6", _instance.VehicleModel);
        }

        PitStop _machine;
        PitStopInstance _instance;

        [OneTimeSetUp]
        public void Setup()
        {
            _machine = new PitStop();
            _instance = new PitStopInstance();

            var vehicle = new Vehicle
            {
                Make = "Audi",
                Model = "A6",
            };

            _machine.RaiseEvent(_instance, _machine.VehicleArrived, vehicle).Wait();
        }


        class PitStopInstance
        {
            public State OverallState { get; private set; }
            public State FuelState { get; private set; }
            public State OilState { get; private set; }

            public string VehicleMake { get; set; }
            public string VehicleModel { get; set; }

            public decimal FuelGallons { get; set; }
            public decimal FuelPricePerGallon { get; set; }
            public decimal FuelCost { get; set; }

            public decimal OilQuarts { get; set; }
            public decimal OilPricePerQuart { get; set; }
            public decimal OilCost { get; set; }
        }


        class PitStop :
            AutomatonymousStateMachine<PitStopInstance>
        {
            public PitStop()
            {
                InstanceState(x => x.OverallState);

                During(Initial,
                    When(VehicleArrived)
                        .Then(context =>
                        {
                            context.Instance.VehicleMake = context.Data.Make;
                            context.Instance.VehicleModel = context.Data.Model;
                        })
                        .TransitionTo(BeingServiced)
//                        .RunParallel(p =>
//                            {
//                                p.Start<FillTank>(x => x.BeginFilling);
//                                p.Start<CheckOil>(x => x.BeginChecking);
//                            }))
                    );
            }

            public State BeingServiced { get; private set; }

            public Event<Vehicle> VehicleArrived { get; private set; }
        }


        class FillTank :
            AutomatonymousStateMachine<PitStopInstance>
        {
            public FillTank()
            {
                InstanceState(x => x.FuelState);

                Initially(
                    When(Initial.Enter)
                        .TransitionTo(Filling));

                During(Filling,
                    When(Full)
                        .Then(context =>
                        {
                            context.Instance.FuelGallons = context.Data.Gallons;
                            context.Instance.FuelPricePerGallon = context.Data.PricePerGallon;
                            context.Instance.FuelCost = context.Data.Gallons * context.Data.PricePerGallon;
                        })
                        .Finalize());
            }

            public State Filling { get; private set; }

            public Event<FuelDispensed> Full { get; private set; }
        }


        class CheckOil :
            AutomatonymousStateMachine<PitStopInstance>
        {
            public CheckOil()
            {
                InstanceState(x => x.OilState);

                Initially(
                    When(Initial.Enter)
                        .TransitionTo(AddingOil));

                During(AddingOil,
                    When(Done)
                        .Then(context =>
                        {
                            context.Instance.OilQuarts = context.Data.Quarts;
                            context.Instance.OilPricePerQuart = context.Data.PricePerQuart;
                            context.Instance.OilCost = Math.Ceiling(context.Data.Quarts) * context.Data.PricePerQuart;
                        })
                        .Finalize());
            }

            public State AddingOil { get; private set; }

            public Event<OilAdded> Done { get; private set; }
        }


        class FuelDispensed
        {
            public decimal Gallons { get; set; }
            public decimal PricePerGallon { get; set; }
        }


        class OilAdded
        {
            public decimal Quarts { get; set; }
            public decimal PricePerQuart { get; set; }
        }


        class Vehicle
        {
            public string Make { get; set; }
            public string Model { get; set; }
        }
    }
}