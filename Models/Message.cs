using System;

namespace API.Models;

public class Message
{
    public int Id { get; set; }
    public string? SenderId { get; set; }
    public string?ReceiverId { get; set; }
    public string? content { get; set; }

    public DateTime CreateDate { get; set; }

    public bool IsRead { get; set; }
    public AppUser? Sender { get; set; }

    public AppUser?Receiver { get; set; }

}
