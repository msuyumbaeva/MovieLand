using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Controllers
{
    public class ControllerBase : Controller
    {
        protected void AddErrors(ICollection<string> errors) {
            foreach (var error in errors) {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
