using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using QBittorrent.Client;

namespace QBittorrent.CommandLineInterface.ViewModels.ServerPreferences
{
    public readonly struct EmailViewModel
    {
        private readonly Preferences _wrappedObject;

        public EmailViewModel(Preferences wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        [Display(Name = "E-mail notifications enabled")]
        public bool? MailNotificationEnabled => _wrappedObject.MailNotificationEnabled;

        [Display(Name = "Send e-mail from")]
        [DisplayFormat(NullDisplayText = "n/a")]
        public string MailNotificationSender => _wrappedObject.MailNotificationSender;

        [Display(Name = "Send e-mail to")]
        public string MailNotificationEmailAddress => _wrappedObject.MailNotificationEmailAddress;

        [Display(Name = "SMTP server")]
        public string MailNotificationSmtpServer => _wrappedObject.MailNotificationSmtpServer;

        [Display(Name = "Use SSL")]
        public bool? MailNotificationSslEnabled => _wrappedObject.MailNotificationSslEnabled;

        [Display(Name = "SMTP server authentication")]
        public bool? MailNotificationAuthenticationEnabled => _wrappedObject.MailNotificationAuthenticationEnabled;

        [Display(Name = "SMTP server username")]
        public string MailNotificationUsername => _wrappedObject.MailNotificationUsername;

        [Display(Name = "SMTP server password")]
        [DisplayFormat(DataFormatString = "**********")]
        public string MailNotificationPassword => _wrappedObject.MailNotificationPassword;
    }
}
