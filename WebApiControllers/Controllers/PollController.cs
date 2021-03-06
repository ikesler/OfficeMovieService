﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Svitla.MovieService.Core.Entities;
using Svitla.MovieService.Core.Helpers;
using Svitla.MovieService.DomainApi;
using Svitla.MovieService.WebApi.Dto;

namespace Svitla.MovieService.WebApi.Controllers
{
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
            var poll = pollFacade.GetCurrent();
            object dto = CreatePollDto(poll);
            return Response(dto);
        }

        [HttpPost]
        [Authorize]
        public EmptyResponseObject CancelCurrent()
        {
            pollFacade.CancelCurrent();

            return Response();
        }

        [Authorize]
        public ResponseObject<object> Save(Poll poll)
        {
            pollFacade.Save(poll);
            return Response(CreatePollDto(poll));
        }

        [HttpPost]
        [Authorize]
        public EmptyResponseObject Vote(EntityId id)
        {
            Vote(id.Id, true);
            return Response();
        }

        [HttpPost]
        [Authorize]
        public EmptyResponseObject Unvote(EntityId id)
        {
            Vote(id.Id, false);
            return Response();
        }
        
        [HttpPost]
        public ResponseObject<object> GetPollMovies()
        {
            object result = null;
            var poll = pollFacade.GetCurrent();
            if (poll != null)
            {   
                var movies = movieFacade.FindMoviesForPoll(poll.Id);
                result = new
                {
                    Poll = new {
                        poll.Id,
                        poll.Name,
                        OwnerName = poll.Owner.Name,
                        poll.ExpirationDate
                    },
                    Movies = movies.Select(m => new
                    {
                        m.Id,
                        m.Name,
                        m.Url,
                        Voters = m.Votes.Where(v => v.PollId == poll.Id).Select(v => v.User.Name)
                    }).ToList()
                };
            }
            return Response(result);
        }

        private void Vote(long movieId, bool isSelected)
        {
            var currentUser = userFacade.GetByEmail(User.Identity.Name);
            var movie = movieFacade.LoadById(movieId);
            pollFacade.Vote(currentUser, movie, isSelected);
        }

        private object CreatePollDto(Poll poll)
        {
            var currentUser = userFacade.GetByEmail(User.Identity.Name);
            object dto = null;
            if (poll != null)
            {
                dto = new
                {
                    poll.CreatedDate,
                    poll.ExpirationDate,
                    poll.Id,
                    poll.IsVoteable,
                    poll.Name,
                    poll.ViewDate,
                    Winner = poll.Winner.Get(m => new { m.Id, m.Name }),
                    IsMine = poll.Owner == currentUser
                };
            }
            return dto;
        }
    }
}
