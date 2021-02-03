using AsteraX.Domain.SharedKernel;

namespace AsteraX.Application.SharedKernel
{
    public abstract class PresenterDomainObserver<TPresenter, TEvent> : IDomainObserver<TEvent>
        where TEvent: IDomainEvent
        where TPresenter: IPresenter
    {
        protected PresenterDomainObserver(TPresenter presenter)
        {
            Presenter = presenter;
        }

        protected TPresenter Presenter { get; }

        public void Subscribe()
        {
            DomainSubject<TEvent>.Subscribe(this);
        }

        public void Unsubscribe()
        {
            DomainSubject<TEvent>.Unsubscribe(this);
        }

        public abstract void Update(TEvent domainEvent);
    }
}