﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.HyperMedia
{
    public class HyperMediaFilter : ResultFilterAttribute
    {
        private readonly HyperMediaFilterOptions _hyperMediaFilterOptions;

        public HyperMediaFilter(HyperMediaFilterOptions hyperMediaFilterOptions) {
            _hyperMediaFilterOptions = hyperMediaFilterOptions;
        }

        public override void OnResultExecuting(ResultExecutingContext context) {
            TryEnrichResult(context);
            base.OnResultExecuting(context);
        }

        private void TryEnrichResult(ResultExecutingContext context) {
            if (context.Result is ObjectResult objectResult) {
                if (objectResult.Value is ISupportsHyperMedia model) {
                    var enricher = _hyperMediaFilterOptions.ObjectContentResponseEnricherList.FirstOrDefault(x => x.CanEnrich(context));
                    if (enricher != null)
                        Task.FromResult(enricher.Enrich(context));
                }
            }
        }
    }
}
