using Microsoft.AspNetCore.SignalR;

namespace WebFilterApplication
{
    public class HubFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            Console.WriteLine($"Hub method: {context.HubMethodName}");
            var arguments = context.HubMethodArguments.ToArray();

            string name = arguments[0].ToString();
            if (name.Length < 5)
            {

                for (int i = name.ToString().Length; i <= 5; i++)
                    name += "*";
                arguments[0] = name;

                context = new HubInvocationContext(
                    context.Context,
                    context.ServiceProvider,
                    context.Hub,
                    context.HubMethod,
                    arguments);
            }

            try
            {
                return await next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public Task OnConnectedAsync(
            HubLifetimeContext context,
            Func<HubLifetimeContext, Task> next)
        {
            Console.WriteLine("On connected method");
            return next(context);
        }

        public Task OnDescinnectedAsync(
            HubLifetimeContext context,
            Exception exception,
            Func<HubLifetimeContext, Exception, Task> next)
        {
            Console.WriteLine("On disconnected method");
            return next(context, exception);
        }
    }
}
