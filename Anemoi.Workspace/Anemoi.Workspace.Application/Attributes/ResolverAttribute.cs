namespace Anemoi.Workspace.Application.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ResolverAttribute(string fieldName) : Attribute
{
    public string FieldName { get; } = fieldName;
}