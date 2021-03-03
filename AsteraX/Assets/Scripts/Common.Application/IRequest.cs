using UniRx;

namespace Common.Application
{
    public interface IBaseRequest { }

    public interface IRequest<out TResponse> : IBaseRequest { }
    
    public interface IRequest : IRequest<Unit> { }
}