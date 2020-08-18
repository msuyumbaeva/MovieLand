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
    public class MovieResultSetEnricher : ObjectContentResponseEnricher<ResultSet<MovieListItemDto>>
    {
        public override Task EnrichModel(ResultSet<MovieListItemDto> content, IUrlHelper urlHelper) {
            if (content.Methadata.Count > content.Count + content.Methadata.Offset)
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetMovies", new {
                        content.Methadata.Limit,
                        Offset = content.Methadata.Offset + content.Methadata.Limit
                    }),
                    Rel = RelationType.next
                });

            if (content.Methadata.Offset > 0) {
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetMovies", new {
                        content.Methadata.Limit,
                        Offset = Math.Max(content.Methadata.Offset - content.Methadata.Limit, 0)
                    }),
                    Rel = RelationType.previous
                });
            }

            content.Links.Add(new HyperMediaLink() {
                Action = HttpActionVerb.GET,
                Href = urlHelper.Link("GetMovies", new {
                    search = "SearchParameter",
                    genre = Guid.Empty,
                    country = Guid.Empty,
                    artist = Guid.Empty,
                    limit = 10,
                    offset = 0
                }),
                Rel = RelationType.self
            });
            return null;
        }
    }
}
