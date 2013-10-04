﻿using System.Web.Http;
using Svitla.MovieService.Core.Entities;
using Svitla.MovieService.DomainApi;
using Svitla.MovieService.WebApi.Dto;

namespace Svitla.MovieService.WebApi.Controllers
{
    [Authorize]
    public class PollController : BaseApiController
    {
        private readonly IPollFacade pollFacade;
        private readonly IUserFacade userFacade;
        private readonly IMovieFacade movieFacade;

        public PollController(IPollFacade pollFacade, IUserFacade userFacade, IMovieFacade movieFacade)
        {
            this.pollFacade = pollFacade;
            this.userFacade = userFacade;
            this.movieFacade = movieFacade;
        }

        [HttpPost]
        public ResponseObject<object> GetCurrent()
        {
            var currentUser = userFacade.GetByEmail(User.Identity.Name);
            var poll = pollFacade.GetCurrent();
            var dto = new
            {
                poll.CreatedDate,
                poll.ExpirationDate,
                poll.Id,
                poll.IsVoteable,
                poll.Name,
                poll.ViewDate,
                poll.Winner,
                IsMine = poll.Owner == currentUser
            };
            return Response<object>(dto);
        }

        public EmptyResponseObject Save(Poll poll)
        {
            var currentUser = userFacade.GetByEmail(User.Identity.Name);
            poll.Owner = currentUser;
            pollFacade.Save(poll);
            return Response();
        }

        [HttpPost]
        public EmptyResponseObject Vote(EntityId id)
        {
            Vote(id.Id, true);
            return Response();
        }

        [HttpPost]
        public EmptyResponseObject Unvote(EntityId id)
        {
            Vote(id.Id, false);
            return Response();
        }

        private void Vote(long movieId, bool isSelected)
        {
            var currentUser = userFacade.GetByEmail(User.Identity.Name);
            var movie = movieFacade.LoadById(movieId);
            pollFacade.Vote(currentUser, movie, isSelected);
        }
    }
}
