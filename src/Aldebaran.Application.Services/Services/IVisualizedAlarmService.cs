﻿using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IVisualizedAlarmService
    {
        Task AddAsync(VisualizedAlarm visualizedAlarm, CancellationToken ct = default);            
    }

}
