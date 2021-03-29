using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

using UnityEngine;
using Newtonsoft.Json;

using Aci.Unity.Models;

namespace Aci.Unity.Networking
{
    public class MailNotifier : MonoBehaviour
    {
        public string configFilename;

        private MailConfig _config;
        private SmtpClient _mailClient;

        private void Start()
        {
            _config = LoadConfig();
            SetupMail();
        }

        private MailConfig LoadConfig()
        {
            using (var reader = new StreamReader(configFilename))
                using (var json = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<MailConfig>(json);
                }
        }

        private void SetupMail()
        {
            _mailClient = new SmtpClient(_config.Servername, int.Parse(_config.Port));

            if (_config.SSL)
                _mailClient.EnableSsl = true;

            _mailClient.Credentials = new NetworkCredential(_config.Username, _config.Password);
        }

        private MailMessage CreateMail()
        {
            if (!_config.Sender.Contains("@"))
                return null;

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_config.Sender);

                switch (_config.Priority)
                {
                    case "high":
                        mail.Priority = MailPriority.High;
                        break;
                    case "medium":
                        mail.Priority = MailPriority.Normal;
                        break;
                    case "low":
                        mail.Priority = MailPriority.Low;
                        break;
                    default:
                        mail.Priority = MailPriority.Normal;
                        break;
                }

                mail.Sender = new MailAddress(_config.Sender);

                foreach (string recipient in _config.Recipient)
                {
                    if (recipient != "")
                        mail.To.Add(recipient);
                }

                if (_config.Copy != null && _config.Copy.Count > 0)
                {
                    foreach (string copy in _config.Copy)
                    {
                        if (copy != "")
                            mail.CC.Add(copy);
                    }
                }

                if (_config.Blindcopy != null && _config.Blindcopy.Count > 0)
                {
                    foreach (string blindcopy in _config.Blindcopy)
                    {
                        if (blindcopy != "")
                            mail.Bcc.Add(blindcopy);
                    }
                }

                mail.Subject = _config.Subject;
                mail.Body = _config.Message;

                return mail;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void SendMail()
        {
            _mailClient?.Send(CreateMail());
        }
    }
}
