﻿using Aldebaran.Application.Services;

namespace Aldebaran.Web.Models.ViewModels
{
    public class Alarm
    {
        public int AlarmId { get; set; }
        public string AlarmMessage { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNumber { get; set; }

        public static async Task<List<Alarm>> GetAlarmsListAsync(List<Application.Services.Models.Alarm> alarms, IAlarmService alarmService, CancellationToken ct = default)
        {
            var result = new List<Alarm>();

            foreach (var alarm in alarms)
            {
                var documentNumber = await alarmService.GetDocumentNumberAsync(alarm.DocumentId, alarm.AlarmMessage.AlarmType.DocumentType.DocumentTypeCode, ct);

                var alarmUser = new Alarm
                {
                    AlarmId = alarm.AlarmId,
                    AlarmMessage = alarm.AlarmMessage.Message,
                    CreationDate = alarm.CreationDate,
                    ExecutionDate = alarm.ExecutionDate,
                    DocumentTypeName = alarm.AlarmMessage.AlarmType.DocumentType.DocumentTypeName,
                    DocumentNumber = documentNumber
                };

                result.Add(alarmUser);
            }

            return result;
        }
    }
}
