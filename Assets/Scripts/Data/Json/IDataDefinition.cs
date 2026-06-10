namespace Skybound.Data.Json
{
    public interface IDataDefinition
    {
        string Id { get; }

        bool IsValid(out string errorMessage);
    }
}