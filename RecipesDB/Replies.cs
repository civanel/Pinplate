using System;

namespace RecipesDB;

public class Replies
{
    public int ReplyId { get; set; }
    public int ComplaintId { get; set; }
    public string RecipientName { get; set; } = "";
    public string ComplaintTitle { get; set; } = "";
    public string Message { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    // Навигационное свойство
    public Complaints Complaint { get; set; } = null!;
}