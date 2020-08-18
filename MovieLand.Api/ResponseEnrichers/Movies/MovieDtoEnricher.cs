using Microsoft.AspNetCore.Mvc;
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
                Href = urlHelper.Link("GetMovie", content.Data.Id),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMoviePoster", content.Data.Id),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovieComments", new { content.Data.Id, limit = 10, offset = 0 }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = urlHelper.Link("PostMovieComment", content.Data.Id),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovieStarRating", content.Data.Id),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = urlHelper.Link("PostMovieStarRating", content.Data.Id),
                Rel = RelationType.self
            });
            return null;
        }
    }
}
