namespace Domain.AuditLogs;

public class AuditLog
{
    public long Id { get; private set; }
    public string Log { get; private set; }

    private AuditLog() { }

    public AuditLog(string log)
    {
        Log = log;
    }
}
