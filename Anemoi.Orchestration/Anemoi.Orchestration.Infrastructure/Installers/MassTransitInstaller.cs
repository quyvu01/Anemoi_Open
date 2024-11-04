using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.Orchestration.Contract.EmailSendingContract.Instances;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;
using Anemoi.Orchestration.Contract.SchedulerContract.Instances;
using Anemoi.Orchestration.Contract.SecureContract.Instances;
using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using Anemoi.Orchestrator.Infrastructure.DataContext;
using Anemoi.Orchestrator.Infrastructure.MachinesDefinition.EmailSending;
using Anemoi.Orchestrator.Infrastructure.MachinesDefinition.Notification;
using Anemoi.Orchestrator.Infrastructure.MachinesDefinition.Orchestrator;
using Anemoi.Orchestrator.Infrastructure.MachinesDefinition.Scheduler;
using Anemoi.Orchestrator.Infrastructure.MachinesDefinition.Workspace;
using Anemoi.Orchestrator.Machine.Machines.EmailSendingMachines;
using Anemoi.Orchestrator.Machine.Machines.NotificationMachines;
using Anemoi.Orchestrator.Machine.Machines.SchedulerMachines;
using Anemoi.Orchestrator.Machine.Machines.SecureMachines;
using Anemoi.Orchestrator.Machine.Machines.WorkspaceMachines;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Orchestrator.Infrastructure.Installers;

public sealed class MassTransitInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var masstransitSetting = configuration.GetSection(nameof(MassTransitSetting)).Get<MassTransitSetting>()!;
        var (host, virtualHost, userName, password, _, _) = masstransitSetting;
        var dbSetting = configuration.GetSection(nameof(MongoDbSetting)).Get<MongoDbSetting>()!;
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator
                .AddSagaStateMachine<EmailSendingRelayMachine, EmailSendingRelayInstance,
                    EmailSendingRelayDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<NotificationHubMachine, NotificationHubInstance,
                    NotificationHubDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<NotificationReminderMachine, NotificationReminderInstance,
                    NotificationReminderDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<SchedulerMachine, SchedulerInstance,
                    SchedulerDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<OtpOperationMachine, OtpOperationInstance,
                    OtpOperationDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<WorkspaceInitializedMachine, WorkspaceInitializedInstance,
                    WorkspaceInitializedDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator
                .AddSagaStateMachine<MemberRoleGroupSynchronizedMachine, MemberRoleGroupSynchronizedInstance,
                    MemberRoleGroupSynchronizedDefinition>()
                .MongoDbRepository(cfg =>
                {
                    cfg.Connection = dbSetting.ConnectionString;
                    cfg.DatabaseName = dbSetting.MongoDatabase;
                });

            configurator.AddEntityFrameworkOutbox<OrchestrationDbContext>(o =>
            {
                o.UsePostgres().UseBusOutbox();
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(10);
            });
            configurator.AddDelayedMessageScheduler();

            configurator.UsingRabbitMq((context, bus) =>
            {
                bus.Host(host, virtualHost, c =>
                {
                    c.Username(userName);
                    c.Password(password);
                });
                bus.ConfigureEndpoints(context);
                bus.UseDelayedMessageScheduler();
            });
        });
    }
}