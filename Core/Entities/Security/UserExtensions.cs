﻿using System.Linq;
using Svitla.MovieService.Core.Helpers;

namespace Svitla.MovieService.Core.Entities.Security
{
    public static class UserExtensions
    {
        public static bool HasPermission(this User user, Permissions permission)
        {
            return user.Get(u => u.Permissions.Any(p => p.Code == permission));
        }
    }
}