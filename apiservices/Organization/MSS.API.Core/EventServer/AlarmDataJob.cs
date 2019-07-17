using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Quartz;
namespace MSS.API.Core.EventServer
{
    public class AlarmDataJob : IJob
    {
        // private readonly IDemoService _demoService;
        // private readonly DemoJobOptions _options;
        private readonly ILogger _logger;
        public AlarmDataJob(ILogger<AlarmDataJob> logger/* IDemoService demoService,  *//* IOptions<DemoJobOptions> options */)
        {
            // _demoService = demoService;
            // _options = options.Value;
            _logger = logger;
        }

        public  Task Execute(IJobExecutionContext context)
        {
            // _demoService.DoTask(_options.Url);
            _logger.LogInformation("test");
            // await Console.Out.WriteLineAsync("Greetings from HelloJob!");
            return Task.CompletedTask;
        }
    }
}