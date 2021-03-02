using UniRx;

namespace AsteraX.Application.Common
{
    public interface IBaseRequest { }

    public interface IRequest<out TResponse> : IBaseRequest { }
    
    public interface IRequest : IRequest<Unit> { }
}