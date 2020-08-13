using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.IdentityServer
{
    public class ConfigurationManager
    {
        public string Hash { get; set; }
        public string hgAuthSecret { get; set; }
        public string AppSecret { get; set; }
        public string defaultAdminPsw { get; set; }
        public string ClientId { get; set; }
        public string ApiName { get; set; }
    }
}
