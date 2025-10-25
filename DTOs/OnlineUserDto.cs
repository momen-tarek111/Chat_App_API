using System;

namespace API.DTOs;

public class OnlineUserDto
{
    public string? Id { get; set; }

    public string? ConnectionId { get; set; }
    public string? UserName { get; set; }

    public string?FullName { get; set; }

    public string? ProfileImage { get; set; }

    public bool isOnline { get; set; }

    public int UnreadCount { get; set; }
}
