namespace Device.Fingerprint.Core
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;

    public static class JsonSerializerSettingsFactory
    {
        private static volatile JsonSerializerSettings _instance = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static JsonSerializerSettings Instance => _instance;
    }
}
