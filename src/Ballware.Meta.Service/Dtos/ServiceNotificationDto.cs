﻿namespace Ballware.Meta.Service.Dtos;

public class ServiceNotificationDto
{
    public Guid Id { get; set; }

    public string? Identifier { get; set; }

    public string? Name { get; set; }
    public Guid? DocumentId { get; set; }
    public int State { get; set; }
    public string? Params { get; set; }
}