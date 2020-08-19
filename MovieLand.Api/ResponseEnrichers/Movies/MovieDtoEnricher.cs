﻿using Microsoft.AspNetCore.Mvc;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.ResponseEnrichers
{
    public class MovieDtoEnricher : ObjectContentResponseEnricher<HyperMediaLinksDecorator<MovieDto>>
    {
        public override Task EnrichModel(HyperMediaLinksDecorator<MovieDto> content, IUrlHelper urlHelper) {
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovie", new { content.Data.Id }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMoviePoster", new { content.Data.Id }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovieComments", new { content.Data.Id, limit = 10, offset = 0 }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = urlHelper.Link("PostMovieComment", new { content.Data.Id }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovieStarRating", new { content.Data.Id }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = urlHelper.Link("PostMovieStarRating", new { content.Data.Id }),
                Rel = RelationType.self
            });
            return null;
        }
    }
}
