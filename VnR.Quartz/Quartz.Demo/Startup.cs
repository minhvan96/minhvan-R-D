using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Demo.JobListeners;
using Quartz.Demo.Jobs;
using Quartz.Demo.SchedulerListeners;
using Quartz.Demo.TriggerListeners;
using Quartz.Impl.Matchers;
using Serilog;
using System;

namespace Quartz.Examples.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Logging

            // make sure you configure logging and open telemetry before quartz services
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddRazorPages();

            #endregion Logging

            #region Quartz

            // base configuration for DI
            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
            services.AddQuartz(q =>
            {
                // handy when part of cluster or you want to otherwise identify multiple schedulers
                q.SchedulerId = "Scheduler-Core";

                #region DI Job Factory

                // this is default configuration if you don't alter it
                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    // if we don't have the job in DI, allow fallback to configure via default constructor
                    options.AllowDefaultConstructor = true;

                    // set to true if you want to inject scoped services like Entity Framework's DbContext
                    options.CreateScope = false;
                });

                #endregion DI Job Factory

                #region Default Config -- For InMemory Store -- Comment if uses Persist Store -- Commented

                // these are the defaults
                //q.UseSimpleTypeLoader();
                //q.UseInMemoryStore();
                //q.UseDefaultThreadPool(tp =>
                //{
                //    tp.MaxConcurrency = 10;
                //});

                #endregion Default Config -- For InMemory Store -- Comment if uses Persist Store -- Commented

                #region Simple Trigger

                //// Create a job with Single Trigger
                //q.ScheduleJob<FirstSample>(trigger => trigger
                //    .WithIdentity("SomeTrigger")
                //    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(5)))
                //    .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                //    .WithDescription("Say somethign Trigger configured for a SaySomethingJob with single call")
                //    );

                #endregion Simple Trigger

                #region Configure Individual Jobs And Trigger -- Commented

                // you can also configure individual jobs and triggers with code
                // this allows you to associated multiple triggers with same job
                // (if you want to have different job data map per trigger for example)
                //q.AddJob<ExampleJob>(j => j
                //    .StoreDurably() // we need to store durably if no trigger is associated
                //    .WithDescription("my awesome job")
                //);

                #endregion Configure Individual Jobs And Trigger -- Commented

                #region Second Sample Job with SampleTrigger and CronTrigger

                var secondSampleJobKey = new JobKey("Second Sample Job", "Sample Job Group");
                q.AddJob<SecondSample>(secondSampleJobKey, j => j.WithDescription("My Second Sample Job"));

                // Sample Trigger for Second Sample Job
                q.AddTrigger(trigger => trigger
                    .WithIdentity("SecondSample Sample Trigger")
                    .ForJob(secondSampleJobKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(7)).RepeatForever())
                    .WithDescription("My Second Sample Trigger")
                    );

                // Cron Trigger for Second Sample Job
                q.AddTrigger(trigger => trigger
                    .WithIdentity("My Second Sample Cron Trigger")
                    .ForJob(secondSampleJobKey)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                    .WithCronSchedule("0/17 * * * * ?")
                    .WithDescription("My Second Cron Trigger")
                    );

                #endregion Second Sample Job with SampleTrigger and CronTrigger

                #region TimeConverter

                q.UseTimeZoneConverter();

                #endregion TimeConverter

                #region Sample Listener

                // Add Second Sample Listener
                q.AddJobListener<SecondSampleJobListener>(GroupMatcher<JobKey>.GroupEquals(secondSampleJobKey.Group));
                q.AddTriggerListener<SecondSampleTriggerListener>();
                q.AddSchedulerListener<SecondSampleSchedulerListener>();

                #endregion Sample Listener

                #region Job Using Data

                var dataJobKey = new JobKey("AuthorInfo", "Using Data Group");
                q.AddJob<UsingDataJob>(options =>
                    options.WithIdentity(dataJobKey)
                        .UsingJobData("Name", "Nguyen Minh Van")
                        .UsingJobData("Age", "24")
                        .UsingJobData("Address", "District 12, Ho Chi Minh City")
                        );

                q.AddTrigger(options =>
                    options.ForJob(dataJobKey)
                        .WithIdentity("DataJob-Trigger")
                        .WithCronSchedule("0/5 * * * * ?"));

                #endregion Job Using Data

                #region Calender

                //const string calendarName = "myHolidayCalendar";
                //q.AddCalendar<HolidayCalendar>(
                //    name: calendarName,
                //    replace: true,
                //    updateTriggers: true,
                //    x => x.AddExcludedDate(new DateTime(2020, 5, 15))
                //);

                //q.AddTrigger(t => t
                //    .WithIdentity("Daily Trigger")
                //    .ForJob(jobKey)
                //    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(5)))
                //    .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                //    .WithDescription("my awesome daily time interval trigger")
                //    .ModifiedByCalendar(calendarName)
                //);

                // convert time zones using converter that can handle Windows/Linux differences
                //q.UseTimeZoneConverter();

                #endregion Calender
            });

            #endregion Quartz

            #region Quartz Options

            // we can use options pattern to support hooking your own configuration with Quartz's
            // because we don't use service registration api, we need to manally ensure the job is present in DI
            //services.AddTransient<ExampleJob>();

            //services.Configure<SampleOptions>(Configuration.GetSection("Sample"));
            //services.AddOptions<QuartzOptions>()
            //    .Configure<IOptions<SampleOptions>>((options, dep) =>
            //    {
            //        if (!string.IsNullOrWhiteSpace(dep.Value.CronSchedule))
            //        {
            //            var jobKey = new JobKey("options-custom-job", "custom");
            //            options.AddJob<ExampleJob>(j => j.WithIdentity(jobKey));
            //            options.AddTrigger(trigger => trigger
            //                .WithIdentity("options-custom-trigger", "custom")
            //                .ForJob(jobKey)
            //                .WithCronSchedule(dep.Value.CronSchedule));
            //        }
            //    });

            #endregion Quartz Options

            #region Quartz Server

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            #endregion Quartz Server

            #region Other Services

            services
                .AddHealthChecksUI()
                .AddInMemoryStorage();

            #endregion Other Services
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHealthChecksUI();
            });
        }
    }
}