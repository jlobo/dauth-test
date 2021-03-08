using System;

namespace dAuthMe.api
{
    public class Settings
    {
        public MongoSettings Mongo { get; set; }
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}