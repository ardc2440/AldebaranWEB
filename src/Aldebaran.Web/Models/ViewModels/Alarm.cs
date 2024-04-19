using Aldebaran.Application.Services;

namespace Aldebaran.Web.Models.ViewModels
{
    public class Alarm
    {
        public int AlarmId { get; set; }
        public String AlarmMessage { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNumber { get; set; }

        public static List<Alarm> GetAlarmsList(List<Application.Services.Models.Alarm> alarms, IAlarmService alarmService)
        {
            var result = new List<Alarm>();

            foreach (var alarm in alarms)
            {
                var documentNumber = alarmService.GetDocumentNumber(alarm.DocumentId, alarm.AlarmMessage.AlarmType.DocumentType.DocumentTypeCode);

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
