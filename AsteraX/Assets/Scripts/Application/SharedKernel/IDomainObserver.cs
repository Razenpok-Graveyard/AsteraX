using AsteraX.Domain.SharedKernel;

namespace AsteraX.Application.SharedKernel
{
    public interface IDomainObserver
    {
        void Subscribe();
        void Unsubscribe();
    }

    public interface IDomainObserver<in T> where T: IDomainEvent
    {
        void Update(T domainEvent);
    }
}