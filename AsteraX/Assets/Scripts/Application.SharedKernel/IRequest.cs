using UniRx;

namespace AsteraX.Application.SharedKernel
{
    public interface IRequest : IRequest<Unit> { }

    public interface IRequest<out TResponse> : IBaseRequest { }

    public interface IBaseRequest { }
}