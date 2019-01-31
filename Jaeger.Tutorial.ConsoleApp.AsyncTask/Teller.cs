using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Tag;

namespace Jaeger.Tutorial.ConsoleApp.AsyncTask
{
    public class Teller
    {
        private readonly string _message;
        private readonly ITracer _tracer;
        private readonly ILogger<Teller> _logger;

        public async Task TellAsync(string message, Teller teller)
        {
            await Task.Factory.StartNew(async () =>
            {
                using (IScope scope = _tracer.BuildSpan("TellAsync").StartActive(true))
                {
                    try
                    {
                        // avoid burst calling func
                        await Task.Delay(10);

                        scope.Span.Log($"{message}");
                        Console.Write($"{message} ");

                        await teller.TellAsync(_message, this);
                    }
                    catch (Exception ex)
                    {
                        Tags.Error.Set(scope.Span, true);
                    }
                }
            });
        }

        public Teller(string message, ITracer tracer, ILoggerFactory loggerFactory)
        {
            _message = message;
            _tracer = tracer;
            _logger = loggerFactory.CreateLogger<Teller>();
        }

        public async Task IceBreakAsync(Teller receiver)
        {
            using (IScope scope = _tracer.BuildSpan(nameof(IceBreakAsync)).StartActive(true))
            {
                await receiver.TellAsync(_message, this);
            }
        }
    }
}
