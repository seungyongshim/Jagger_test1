using System;
using System.Threading.Tasks;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;

namespace Jaeger.Tutorial.ConsoleApp.AsyncTask
{
    internal class Program
    {
        private static Tracer InitTracer(string serviceName, ILoggerFactory loggerFactory)
        {
            Configuration.SenderConfiguration sender = new Configuration.SenderConfiguration(loggerFactory)
               .WithAgentHost("127.0.0.1")
               .WithAgentPort(6831);

            Configuration.SamplerConfiguration samplerConfiguration = new Configuration.SamplerConfiguration(loggerFactory)
                .WithType(ConstSampler.Type)
                .WithParam(1);

            Configuration.ReporterConfiguration reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                .WithSender(sender)
                .WithLogSpans(true);

            return (Tracer)new Configuration(serviceName, loggerFactory)
                .WithSampler(samplerConfiguration)
                .WithReporter(reporterConfiguration)
                .GetTracer();
        }

        private static async Task Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            Tracer tracer = InitTracer("Jaeger.Tutorial.ConsoleApp", loggerFactory);

            var pinger = new Teller("Hello", tracer, loggerFactory);
            var ponger = new Teller("World", tracer, loggerFactory);

            await pinger.IceBreakAsync(ponger); // 먼저 말을 걸기

            Console.In.ReadLine();
        }
    }
}
