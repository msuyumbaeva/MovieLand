using Microsoft.AspNetCore.Mvc;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.BLL.Dtos.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.ResponseEnrichers.Movies
{
    public class CommentResultSetEnricher : ObjectContentResponseEnricher<ResultSet<CommentDto>>
    {
        public override Task EnrichModel(ResultSet<CommentDto> content, IUrlHelper urlHelper) {
            if (content.Methadata.Count > content.Count + content.Methadata.Offset)
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetMovieComments", new {
                        content.Methadata.Limit,
                        Offset = content.Methadata.Offset + content.Methadata.Limit
                    }),
                    Rel = RelationType.next
                });

            if (content.Methadata.Offset > 0) {
                content.Links.Add(new HyperMediaLink() {
                    Action = HttpActionVerb.GET,
                    Href = urlHelper.Link("GetMovieComments", new {
                        content.Methadata.Limit,
                        Offset = Math.Max(content.Methadata.Offset - content.Methadata.Limit, 0)
                    }),
                    Rel = RelationType.previous
                });
            }
            return null;
        }
    }
}
