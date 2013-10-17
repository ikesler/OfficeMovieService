﻿using System;
using System.Collections.Generic;
using Svitla.MovieService.Core.Entities.Security;
using Svitla.MovieService.Core.Logging;

namespace Svitla.MovieService.Core.Entities
{
    public class User : Entity
    {
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        [Log(Verbosity.Empty)]
        public User InvitedBy { get; set; }

        [Log(Verbosity.Full)]
        public virtual ICollection<Vote> Votes { get; set; }

        [Log(Verbosity.Full)]
        public virtual ICollection<Permission> Permissions { get; set; }

        public User()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
