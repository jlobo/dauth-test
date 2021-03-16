using System;
using Microsoft.IdentityModel.Tokens;

namespace dAuthMe.api
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class ApiSettings
    {
        public string SecretKey { get; set; }

        public SymmetricSecurityKey GetSecurityKey() => new SymmetricSecurityKey(Convert.FromBase64String(SecretKey));
    }
}