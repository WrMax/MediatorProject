using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using MediatR.Internal;

namespace MediatorProject
{
    public class CommentRequest : IRequest<CommentResponse>
    {
        public string Message { get; set; }
    }

    public class CommentResponse
    {
        public string Message { get; set; }
    }


    public class CommentHandler : IRequestHandler<CommentRequest, CommentResponse>
    {

        public CommentHandler()
        {
        }

        public CommentResponse Handle(CommentRequest request)
        {
            return new CommentResponse { Message = request.Message + " Pong" };
        }

        public async Task<CommentResponse> HandleAsync(CommentRequest request)
        {
            var value = await Task.Run(() => { return new CommentResponse { Message = request.Message + " Pong" }; });
            return value;
        }
    }

    class RequestProvider
    {
        public object Send(object request)
        {
            // CommentRequest
            var requestType = request.GetType();

            // IRequst<CommentResponse>
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterfaceType == null)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            // CommentResponse
            var responseType = requestInterfaceType.GetGenericArguments()[0];

            // CommentHandler
            var handler = Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType));
            return ((RequestHandlerBase)handler).Handle(request);
        }


        public object SendAsync(object request)
        {
            // CommentRequest
            var requestType = request.GetType();

            // IRequst<CommentResponse>
            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterfaceType == null)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            // CommentResponse
            var responseType = requestInterfaceType.GetGenericArguments()[0];

            // CommentHandler
            var handler = Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType));
            var result = ((RequestHandlerBase)handler).HandleAsync(request).Result;
            return result;
        }

        public async void Send()
        {
            var responseAsync = Send(new CommentRequest() { Message = "Ping" });
            Console.WriteLine(((CommentResponse)responseAsync).Message);
            Console.ReadLine();
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            new RequestProvider().Send();
            return;
        }
    }
}
