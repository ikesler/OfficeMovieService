﻿using System;
using System.Collections.Generic;

namespace Svitla.MovieService.Core.Entities
{
    public class Poll : Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// Date when poll was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Date when poll will be closed.
        /// </summary>
        public DateTimeOffset ExpirationDate { get; set; }

        /// <summary>
        /// Date when movie which has won will be viewed.
        /// </summary>
        public DateTimeOffset ViewDate { get; set; }

        public User Owner { get; set; }
        public ICollection<PollCandidate> Candidates { get; set; }

        public Poll()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
