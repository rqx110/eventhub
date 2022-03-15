using EventHub.EntityFrameworkCore;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace EventHub
{

    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpBackgroundWorkersModule),
        typeof(EventHubEntityFrameworkCoreModule)
    )]
    public class EventHubBackgroundServicesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();

            context.Services.AddHostedService<EventHubBackgroundServicesHostedService>();
            
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "EventHub:";
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<EventReminderWorker>();
            context.AddBackgroundWorker<NewEventWorker>();
            context.AddBackgroundWorker<EventTimingChangeWorker>();
            
            context.AddBackgroundWorker<OrganizationPaidEnrollmentEndDateWorker>();
        }
    }
}
