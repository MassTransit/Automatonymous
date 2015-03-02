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
    namespace Request_Specs
    {
        using System;
        using System.Linq.Expressions;
        using System.Reflection;
        using System.Threading;
        using System.Threading.Tasks;
        using Binders;
        using Events;
        using NUnit.Framework;


        [TestFixture]
        public class Using_a_request_in_a_state_machine
        {
            [Test]
            public async void Should_property_initialize()
            {
                var machine = new TestStateMachine();
                var instance = new TestState();

                var requestQuote = new RequestQuote
                {
                    Symbol = "MSFT",
                    TicketNumber = "8675309",
                };

                ConsumeContext<RequestQuote> consumeContext = new InternalConsumeContext<RequestQuote>(requestQuote);

                await machine.RaiseEvent(instance, machine.QuoteRequested, requestQuote, consumeContext);
            }
        }


        interface Fault<T>
            where T : class
        {
        }


        interface Request<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            Event<TResponse> Completed { get; set; }
            Event<Fault<TRequest>> Faulted { get; set; }
            Event<TRequest> TimeoutExpired { get; set; }

            Task SendRequest<T>(ConsumeContext<T> context, TRequest requestMessage)
                where T : class;
        }


        interface ConsumeContext<T>
            where T : class
        {
            T Message { get; }
        }


        class RequestStateMachine<T> :
            AutomatonymousStateMachine<T>
            where T : class
        {
            protected void Request<TRequest, TResponse>(Expression<Func<Request<TRequest, TResponse>>> propertyExpression,
                Uri serviceAddress, TimeSpan timeout)
                where TRequest : class
                where TResponse : class
            {
                PropertyInfo property = propertyExpression.GetPropertyInfo();

                string requestName = property.Name;

                var request = new StateMachineRequest<TRequest, TResponse>(requestName, serviceAddress, timeout);

                property.SetValue(this, request);

                Event(propertyExpression, x => x.Completed);
                Event(propertyExpression, x => x.Faulted);
                Event(propertyExpression, x => x.TimeoutExpired);
            }
        }


        class StateMachineRequest<TRequest, TResponse> :
            Request<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            readonly Uri _serviceAddress;
            readonly TimeSpan _timeout;

            public StateMachineRequest(string propertyName, Uri serviceAddress, TimeSpan timeout)
            {
                _serviceAddress = serviceAddress;
                _timeout = timeout;

                Completed = new DataEvent<TResponse>(string.Format("{0}.Completed", propertyName));
                Faulted = new DataEvent<Fault<TRequest>>(string.Format("{0}.Faulted", propertyName));
                TimeoutExpired = new DataEvent<TRequest>(string.Format("{0}.TimeoutExpired", propertyName));
            }

            public Event<TResponse> Completed { get; set; }
            public Event<Fault<TRequest>> Faulted { get; set; }
            public Event<TRequest> TimeoutExpired { get; set; }

            public async Task SendRequest<T>(ConsumeContext<T> context, TRequest requestMessage)
                where T : class
            {
                // capture requestId
                // send request to endpoint
                // schedule timeout for requestId
            }
        }


        class InternalConsumeContext<T> :
            ConsumeContext<T>
            where T : class
        {
            readonly T _message;

            public InternalConsumeContext(T message)
            {
                _message = message;
            }

            public T Message
            {
                get { return _message; }
            }
        }


        class GetQuote
        {
            public string Symbol { get; set; }
        }


        class Quote
        {
            public string Symbol { get; set; }
            public decimal Last { get; set; }
            public decimal Bid { get; set; }
            public decimal Ask { get; set; }
        }


        class TestState
        {
            public string TicketNumber { get; set; }
            public int CurrentState { get; set; }
        }


        class RequestQuote
        {
            public string TicketNumber { get; set; }
            public string Symbol { get; set; }
        }


        interface IRequestClient<in TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            Task<TResponse> Request(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
        }


        class TestStateMachine :
            RequestStateMachine<TestState>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => QuoteRequested);
                State(() => QuotePending);

                Request(() => QuoteRequest, new Uri("loopback://localhost/my_queue"), TimeSpan.FromSeconds(30));

                Initially(
                    When(QuoteRequested)
                        .Then(context => Console.WriteLine("Quote requested: {0}", context.Data.Symbol))
                        .Request(QuoteRequest, context => new GetQuote {Symbol = context.Message.Symbol})
                        .TransitionTo(QuotePending));

                During(QuotePending,
                    When(QuoteRequest.Completed)
                        .Then((context) => Console.WriteLine("Request Completed!")),
                    When(QuoteRequest.Faulted)
                        .Then((context) => Console.WriteLine("Request Faulted")),
                    When(QuoteRequest.TimeoutExpired)
                        .Then((context) => Console.WriteLine("Request timed out")));
            }

            public Request<GetQuote, Quote> QuoteRequest { get; set; }

            public State QuotePending { get; set; }

            public Event<RequestQuote> QuoteRequested { get; set; }
        }


        static class RequestExtensions
        {
            public static EventActivityBinder<TInstance, TData> Request<TInstance, TData, TRequest, TResponse>(
                this EventActivityBinder<TInstance, TData> binder, Request<TRequest, TResponse> request,
                Func<ConsumeContext<TData>, TRequest> requestMessageFactory)
                // Action<BehaviorContext<TInstance, TData>> action)
                where TInstance : class
                where TRequest : class
                where TResponse : class where TData : class
            {
                var activity = new RequestActivity<TInstance, TData, TRequest, TResponse>(request, requestMessageFactory);

                return binder.Add(activity);
            }
        }


        class RequestActivity<TInstance, TData, TRequest, TResponse> :
            Activity<TInstance, TData>
            where TRequest : class
            where TResponse : class
            where TData : class
        {
            readonly Request<TRequest, TResponse> _request;
            readonly Func<ConsumeContext<TData>, TRequest> _requestMessageFactory;

            public RequestActivity(Request<TRequest, TResponse> request, Func<ConsumeContext<TData>, TRequest> requestMessageFactory)
            {
                _request = request;
                _requestMessageFactory = requestMessageFactory;
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
            {
                ConsumeContext<TData> consumeContext;
                if (!context.TryGetPayload(out consumeContext))
                    throw new ArgumentException("The ConsumeContext was not available");


                TRequest requestMessage = _requestMessageFactory(consumeContext);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }
        }


        static class ExpressionExtensions
        {
            public static PropertyInfo GetPropertyInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
            {
                return expression.GetMemberExpression().Member as PropertyInfo;
            }

            public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> expression)
            {
                return expression.GetMemberExpression().Member as PropertyInfo;
            }

            public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> expression)
            {
                if (expression == null)
                    throw new ArgumentNullException("expression");

                return GetMemberExpression(expression.Body);
            }

            public static MemberExpression GetMemberExpression<T>(this Expression<Func<T>> expression)
            {
                if (expression == null)
                    throw new ArgumentNullException("expression");
                return GetMemberExpression(expression.Body);
            }

            static MemberExpression GetMemberExpression(Expression body)
            {
                if (body == null)
                    throw new ArgumentNullException("body");

                MemberExpression memberExpression = null;
                if (body.NodeType == ExpressionType.Convert)
                {
                    var unaryExpression = (UnaryExpression)body;
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else if (body.NodeType == ExpressionType.MemberAccess)
                    memberExpression = body as MemberExpression;

                if (memberExpression == null)
                    throw new ArgumentException("Expression is not a member access");

                return memberExpression;
            }
        }
    }
}