namespace Automatonymous.Tests.DynamicModify
{
    namespace Request_Specs
    {
        using System;
        using System.Linq.Expressions;
        using System.Reflection;
        using System.Threading.Tasks;
        using Automatonymous.Builder;
        using Binders;
        using GreenPipes;
        using NUnit.Framework;


        [TestFixture(Category = "Dynamic Modify")]
        public class Using_a_request_in_a_state_machine
        {
            [Test]
            public async Task Should_property_initialize()
            {
                Request<GetQuote, Quote> QuoteRequest = null;
                Event<RequestQuote> QuoteRequested = null;

                var machine = AutomatonymousStateMachine<TestState>
                    .Create(builder => builder
                        .InstanceState(x => x.CurrentState)
                        .Event("QuoteRequested", out QuoteRequested)
                        .Request(x => x.ServiceAddress = new Uri("loopback://localhost/my_queue"), "QuoteRequest", out QuoteRequest)
                        .Initially()
                            .When(QuoteRequested, b => b
                                .Then(context => Console.WriteLine("Quote requested: {0}", context.Data.Symbol))
                                .Request(QuoteRequest, context => new GetQuote { Symbol = context.Message.Symbol })
                                .TransitionTo(QuoteRequest.Pending)
                            )
                            .During(QuoteRequest.Pending)
                                .When(QuoteRequest.Completed, b => b.Then((context) => Console.WriteLine("Request Completed!")))
                                .When(QuoteRequest.Faulted, b => b.Then((context) => Console.WriteLine("Request Faulted")))
                                .When(QuoteRequest.TimeoutExpired, b => b.Then((context) => Console.WriteLine("Request timed out")))
                    );
                var instance = new TestState();

                var requestQuote = new RequestQuote
                {
                    Symbol = "MSFT",
                    TicketNumber = "8675309",
                };

                ConsumeContext<RequestQuote> consumeContext = new InternalConsumeContext<RequestQuote>(requestQuote);

                await machine.RaiseEvent(instance, QuoteRequested, requestQuote, consumeContext);

                await machine.RaiseEvent(instance, QuoteRequest.Completed, new Quote {Symbol = requestQuote.Symbol});
            }
        }


        interface Fault<T>
            where T : class
        {
        }


        interface Request<TRequest, out TResponse>
            where TRequest : class
            where TResponse : class
        {
            /// <summary>
            /// The name of the request
            /// </summary>
            string Name { get; }

            /// <summary>
            /// The event that is raised when the request completes and the response is received
            /// </summary>
            Event<TResponse> Completed { get; }

            /// <summary>
            /// The event raised when the request faults
            /// </summary>
            Event<Fault<TRequest>> Faulted { get; }

            /// <summary>
            /// The event raised when the request times out with no response received
            /// </summary>
            Event<TRequest> TimeoutExpired { get; }

            /// <summary>
            /// The state that is transitioned to once the request is pending
            /// </summary>
            State Pending { get; }
        }


        interface ConsumeContext<out T>
            where T : class
        {
            T Message { get; }
        }


        interface RequestConfigurator<T, TRequest, TResponse>
            where T : class
            where TRequest : class
            where TResponse : class
        {
            Uri ServiceAddress { set; }
            TimeSpan Timeout { set; }
        }


        class StateMachineRequestConfigurator<T, TRequest, TResponse> :
            RequestConfigurator<T, TRequest, TResponse>,
            RequestSettings
            where T : class
            where TRequest : class
            where TResponse : class
        {
            Uri _serviceAddress;
            TimeSpan _timeout;

            public StateMachineRequestConfigurator()
            {
                _timeout = TimeSpan.FromSeconds(30);
            }

            public RequestSettings Settings
            {
                get
                {
                    if (_serviceAddress == null)
                        throw new AutomatonymousException("The ServiceAddress was not specified.");

                    return this;
                }
            }

            public Uri ServiceAddress
            {
                get { return _serviceAddress; }
                set { _serviceAddress = value; }
            }

            public TimeSpan Timeout
            {
                get { return _timeout; }
                set { _timeout = value; }
            }
        }

        static class StateMachineExtensions
        {
            public static StateMachineModifier<TInstance> Request<TInstance, TRequest, TResponse>(this StateMachineModifier<TInstance> modifier,
                Action<RequestConfigurator<TestState, TRequest, TResponse>> configureRequest, string requestName, out Request<TRequest, TResponse> request)
                where TInstance : class
                where TRequest : class
                where TResponse : class
            {
                var configurator = new StateMachineRequestConfigurator<TestState, TRequest, TResponse>();

                configureRequest(configurator);

                modifier.Request(requestName, configurator.Settings, out request);
                return modifier;
            }

            private static void Request<TInstance, TRequest, TResponse>(
                this StateMachineModifier<TInstance> modifier, string requestName,
                RequestSettings settings, out Request<TRequest, TResponse> request)
                where TInstance : class
                where TRequest : class
                where TResponse : class
            {
                var smRequest = new StateMachineRequest<TRequest, TResponse>(requestName, settings);

                modifier.Event("Completed", out Event<TResponse> Completed);
                smRequest.Completed = Completed;

                modifier.Event("Faulted", out Event<Fault<TRequest>> Faulted);
                smRequest.Faulted = Faulted;

                modifier.Event("TimeoutExpired", out Event<TRequest> TimeoutExpired);
                smRequest.TimeoutExpired = TimeoutExpired;

                modifier.State("Pending", out State Pending);
                smRequest.Pending = Pending;

                request = smRequest;
            }
        }


        interface RequestSettings
        {
            /// <summary>
            /// The endpoint address of the service that handles the request
            /// </summary>
            Uri ServiceAddress { get; }

            /// <summary>
            /// The timeout period before the request times out
            /// </summary>
            TimeSpan Timeout { get; }
        }


        class StateMachineRequest<TRequest, TResponse> :
            Request<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
        {
            readonly string _name;
            readonly RequestSettings _settings;

            public StateMachineRequest(string requestName, RequestSettings settings)
            {
                _name = requestName;
                _settings = settings;
            }

            public string Name
            {
                get { return _name; }
            }

            public Event<TResponse> Completed { get; set; }
            public Event<Fault<TRequest>> Faulted { get; set; }
            public Event<TRequest> TimeoutExpired { get; set; }

            public State Pending { get; set; }


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

            public Guid QuoteRequestId { get; set; }
        }


        class RequestQuote
        {
            public string TicketNumber { get; set; }
            public string Symbol { get; set; }
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

                await next.Execute(context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public void Probe(ProbeContext context)
            {
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
