using System;

namespace RecipesDB;

public class Log_Entries
{
    public int LogId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = "";
    public string Action { get; set; } = "";
    public string Status { get; set; } = "";
    public string TargetTitle { get; set; } = "";

    public Log_Entries() { }

    public Log_Entries(int logid,
                       DateTime createdat,
                       string username,
                       string action,
                       string status,
                       string targettitle) 
    { 
        LogId = logid;
        CreatedAt = createdat;
        UserName = username;
        Action = action;
        Status = status; 
        TargetTitle = targettitle;
    }
}