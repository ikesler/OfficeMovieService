﻿using System.Web.Http;
using Svitla.MovieService.Core.ValueObjects;
using Svitla.MovieService.DomainApi;

namespace Svitla.MovieService.WebUI.Api
{
    public class MovieController : ApiController
    {
        private IMovieFacade movieFacade;

        public MovieController(IMovieFacade movieFacade)
        {
            this.movieFacade = movieFacade;
        }
    }
}