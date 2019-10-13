namespace MediatR.Internal
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal abstract class RequestHandlerBase
    {
        public abstract object Handle(object request);
        public abstract Task<object> HandleAsync(object request);
    }

    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract TResponse Handle(IRequest<TResponse> request);
        public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request);
    }

    internal class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override object Handle(object request)
        {
            return Handle((IRequest<TResponse>)request);
        }

        public override async Task<object> HandleAsync(object request)
        {
            var result = await HandleAsync((IRequest<TResponse>)request);
            return (object)result;
        }

        public override TResponse Handle(IRequest<TResponse> request)
        {
            var type = typeof(IRequestHandler<TRequest, TResponse>);

            var handlerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).FirstOrDefault();

            var handler = Activator.CreateInstance(handlerType);

            var response = ((IRequestHandler<TRequest, TResponse>)handler).Handle((TRequest)request);
            return response;
        }

        public async override Task<TResponse> HandleAsync(IRequest<TResponse> request)
        {
            var type = typeof(IRequestHandler<TRequest, TResponse>);

            var handlerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).FirstOrDefault();

            var handler = Activator.CreateInstance(handlerType);

            return await ((IRequestHandler<TRequest, TResponse>)handler).HandleAsync((TRequest)request);
        }
    }
}
