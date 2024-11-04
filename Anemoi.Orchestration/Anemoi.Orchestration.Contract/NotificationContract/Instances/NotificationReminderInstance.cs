using MassTransit;

namespace Anemoi.Orchestration.Contract.NotificationContract.Instances;

public sealed class NotificationReminderInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string NotificationType { get; set; }
    public string CorrelationData { get; set; }
    public TimeSpan DeferAfter { get; set; }
    public string WorkspaceId { get; set; }
    public List<string> TargetUserIds { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public Guid? MonitorTimeoutTokenId { get; set; }
    public string State { get; set; }
    public int Version { get; set; }
}