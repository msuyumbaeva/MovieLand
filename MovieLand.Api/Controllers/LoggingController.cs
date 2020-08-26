using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.Controllers
{
    public abstract class LoggingController<TController> : ControllerBase
    {
        protected readonly ILogger<TController> _logger;

        protected LoggingController(ILogger<TController> logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual void LogErrors(IEnumerable<string> errors) {
            foreach (var error in errors) {
                _logger.LogError(error);
            }
        }
    }
}
