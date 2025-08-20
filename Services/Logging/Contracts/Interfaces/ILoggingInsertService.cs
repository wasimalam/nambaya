namespace Logging.Contracts.Interfaces
{
    public interface ILoggingInsertService
    {
        void InsertRecord(object logEvent);
    }
}
