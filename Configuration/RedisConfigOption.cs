namespace APICacheWithRedis.Configuration {
    public class RedisConfigOptions {
        public required string Url {get;set;}
        public string? Username {get;set;}
        public string? Password {get;set;}

    }
}