using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.BLL.Services
{
    public class AppRoleManager : RoleManager<AppRole>
    {
        public AppRoleManager(IRoleStore<AppRole> store, IEnumerable<IRoleValidator<AppRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<AppRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger) {
        }
    }
}
