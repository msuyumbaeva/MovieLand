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
    public class GenreResultSetEnricher : ObjectContentResponseEnricher<ResultSet<GenreDto>>
    {
        public override Task EnrichModel(ResultSet<GenreDto> content, IUrlHelper urlHelper) {
            if(content.Methadata.Count > content.Count + content.Methadata.Offset)
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetGenres", new {
                        content.Methadata.Limit,
                        Offset = content.Methadata.Offset + content.Methadata.Limit
                    }),
                    Rel = RelationType.next
                });

            if(content.Methadata.Offset > 0) {
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetGenres", new {
                        content.Methadata.Limit,
                        Offset = Math.Max(content.Methadata.Offset - content.Methadata.Limit, 0)
                    }),
                    Rel = RelationType.previous
                });
            }

            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetGenres", new { search = "SearchParameter" }),
                Rel = RelationType.self
            });
            return null;
        }
    }
}
