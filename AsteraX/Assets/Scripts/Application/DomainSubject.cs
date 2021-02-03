using System.Collections.Generic;
using AsteraX.Application.SharedKernel;
using AsteraX.Domain.SharedKernel;

namespace AsteraX.Application
{
    public static class DomainSubject<T> where T: IDomainEvent
    {
        private static readonly List<IDomainObserver<T>> Observers = new List<IDomainObserver<T>>();

        public static void Subscribe(IDomainObserver<T> observer)
        {
            Observers.Add(observer);
        }

        public static void Unsubscribe(IDomainObserver<T> observer)
        {
            Observers.Remove(observer);
        }

        public static void Raise(T domainEvent)
        {
            foreach (var observer in Observers)
            {
                observer.Update(domainEvent);
            }
        }
    }
}