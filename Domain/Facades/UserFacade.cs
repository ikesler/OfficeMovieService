﻿using System;
using System.Linq;
using Svitla.MovieService.Core.Entities;
using Svitla.MovieService.DataAccessApi;
using Svitla.MovieService.Domain.DataObjects;
using Svitla.MovieService.DomainApi;
using Svitla.MovieService.DomainApi.Exceptions;
using Svitla.MovieService.MailingApi;

namespace Svitla.MovieService.Domain.Facades
{
    public class UserFacade : BaseFacade, IUserFacade
    {
        private readonly IUserRepository users;
        private readonly Func<IInviteEmail> inviteEmailFactory;

        //These domains are supported by oAuth
        private readonly string[] domainsAllowedForInvintation = { "gmail.com", "svitla.com" };

        public string AllowedDomain { get; set; }

        public UserFacade(DomainContext domainContext, IUnitOfWork unitOfWork, IUserRepository userRepository, Func<IInviteEmail> inviteEmailFactory)
            : base(unitOfWork, domainContext)
        {
            users = userRepository;
            this.inviteEmailFactory = inviteEmailFactory;
        }

        public virtual User GetByEmail(string email)
        {
            return users.One(q => q.FirstOrDefault(u => u.Name == email));
        }
        
        public virtual void Save(User user)
        {
            var existedUser = GetByEmail(user.Name);
            if (existedUser != null)
            {
                user.Id = existedUser.Id;
            }
            else if (!IsDomainValid(user.Name))
            {
                throw new UserDomainDeniedException(AllowedDomain);
            }
            users[user.Id] = user;
            UnitOfWork.Commit();
        }

        public virtual void InviteFriend(User friend)
        {
            if (domainsAllowedForInvintation.All(d => !friend.Name.EndsWith(d)))
            {
                var domains = domainsAllowedForInvintation.Aggregate("", (s, d) => s + ", " + d);
                throw new UserDomainDeniedException(domains.Substring(2));
            }
            var existingUser = GetByEmail(friend.Name);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException(friend.Name);
            }
            friend.InvitedBy = DomainContext.CurrentUser;
            users.Add(friend);
            UnitOfWork.Commit();

            var email = inviteEmailFactory();
            email.Bind(friend);
            email.Send(new [] { new EmailAddress(friend.Name) });
        }

        private bool IsDomainValid(string email)
        {
            return string.IsNullOrEmpty(AllowedDomain) || email.EndsWith(AllowedDomain);
        }
    }
}
