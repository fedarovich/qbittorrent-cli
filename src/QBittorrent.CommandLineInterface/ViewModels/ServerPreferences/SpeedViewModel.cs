using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct SpeedViewModel
    {
        private readonly Preferences _wrappedObject;

        public SpeedViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "Download limit")]
        public int? DownloadLimit => _wrappedObject.DownloadLimit;

        [Display(Name = "Upload limit")]
        public int? UploadLimit => _wrappedObject.UploadLimit;

        [Display(Name = "Alternative download limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s")]
        public int? AlternativeDownloadLimit => _wrappedObject.AlternativeDownloadLimit;

        [Display(Name = "Alternative upload limit")]
        [DisplayFormat(DataFormatString = "{0:N0} bytes/s")]
        public int? AlternativeUploadLimit => _wrappedObject.AlternativeUploadLimit;

        [Display(Name = "Apply alternative limits with scheduler")]
        public bool? SchedulerEnabled => _wrappedObject.SchedulerEnabled;

        [Display(Name = "  From")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        public DateTime? ScheduleFrom => ToDateTime(_wrappedObject.ScheduleFromHour, _wrappedObject.ScheduleFromMinute);

        [Display(Name = "  To")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        public DateTime? ScheduleTo => ToDateTime(_wrappedObject.ScheduleToHour, _wrappedObject.ScheduleToMinute);

        [Display(Name = "  On day")]
        public SchedulerDay? SchedulerDays => _wrappedObject.SchedulerDays;
        
        [Display(Name = "Apply rate limit to uTP protocol")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? LimitUTPRate => _wrappedObject.LimitUTPRate;

        [Display(Name = "Apply rate limit to TCP overhead")]
        public bool? LimitTcpOverhead => _wrappedObject.LimitTcpOverhead;

        [Display(Name = "Apply rate limit to peers on LAN")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool? LimitLAN => _wrappedObject.LimitLAN;
        
        private DateTime? ToDateTime(in int? hours, in int? minutes)
        {
            if (hours == null || minutes == null)
                return null;

            return new DateTime().AddHours(hours.Value).AddMinutes(minutes.Value);
        }
    }
}
