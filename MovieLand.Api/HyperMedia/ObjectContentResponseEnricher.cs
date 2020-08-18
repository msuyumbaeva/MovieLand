﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.HyperMedia
{
    public abstract class ObjectContentResponseEnricher<T> : IResponseEnricher
    {
        public ObjectContentResponseEnricher() {
        }

        /// <summary>
        /// If the types are matched then the object can be enriched with links
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public virtual bool CanEnrich(Type contentType) {
            return contentType == typeof(T);
        }

        public abstract Task EnrichModel(T content, IUrlHelper urlHelper);

        bool IResponseEnricher.CanEnrich(ResultExecutingContext response) {
            if (response.Result is OkObjectResult okObjectResult)
                return CanEnrich(okObjectResult.Value.GetType());
            return false;
        }

        public async Task Enrich(ResultExecutingContext response) {
            // Get the urlHelper
            var urlHelper = (new UrlHelperFactory()).GetUrlHelper(response);
            if (response.Result is OkObjectResult okObjectResult)
                if (okObjectResult.Value is T model) 
                    await EnrichModel(model, urlHelper);
            await Task.FromResult<object>(null);
        }
    }
}
