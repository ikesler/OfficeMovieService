﻿using System.Collections.Generic;

namespace Svitla.MovieService.MailingApi
{
    public interface IEmailClient
    {
        void Send(string subject, string body, IEnumerable<EmailAddress> to, IEnumerable<EmailAddress> cc, IEnumerable<EmailAddress> bcc, EmailAddress from);
    }
}