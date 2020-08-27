using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Models
{
    public static class ILoggerExtension
    {
        public static void LogError(this ILogger logger, IEnumerable<string> errors) {
            foreach (var error in errors)
                logger.LogError(error);
        }
    }
}
