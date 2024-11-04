using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Notification.Domain.Models;

public sealed class EmailConfiguration : ValueObject
{
    public EmailConfigurationId Id { get; set; }
    public string WorkspaceId { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordBytes { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool IsSslEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string Name { get; set; }
    public string SearchHint { get; set; }
    public DateTime CreatedTime { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}