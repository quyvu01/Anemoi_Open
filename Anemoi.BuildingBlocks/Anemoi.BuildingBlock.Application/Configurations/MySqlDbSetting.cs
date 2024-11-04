namespace Anemoi.BuildingBlock.Application.Configurations;

public sealed class MySqlDbSetting : DbSetting
{
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public int BuildVersion { get; set; }

    public void Deconstruct(out int major, out int minor, out int build, out string connectionString) =>
        (major, minor, build, connectionString) = (MajorVersion, MinorVersion, BuildVersion, ConnectionString);
}