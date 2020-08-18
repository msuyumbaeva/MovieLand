using Microsoft.AspNetCore.Mvc;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.BLL.Dtos.Genre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.ResponseEnrichers
{
    public class GenreDtoEnricher : ObjectContentResponseEnricher<HyperMediaLinksDecorator<GenreDto>>
    {
        public override Task EnrichModel(HyperMediaLinksDecorator<GenreDto> content, IUrlHelper urlHelper) {
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetGenre", content.Data.Id ),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.POST,
                Href = urlHelper.Link("PostGenre", new { controller = "Genres" }),
                Rel = RelationType.self
            });
            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.PUT,
                Href = urlHelper.Link("PutGenre", content.Data.Id ),
                Rel = RelationType.self
            });
            return null;
        }
    }
}
