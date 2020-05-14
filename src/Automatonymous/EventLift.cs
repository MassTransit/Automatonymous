namespace Automatonymous
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface EventLift<in TInstance>
        where TInstance : class
    {
        Task Raise(TInstance instance, CancellationToken cancellationToken = default);
    }


    public interface EventLift<in TInstance, in TData>
        where TInstance : class
    {
        Task Raise(TInstance instance, TData data, CancellationToken cancellationToken = default);
    }
}
