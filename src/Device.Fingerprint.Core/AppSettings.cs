using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Device.Fingerprint.Core
{
    public interface IAppSettings
    {
        string ApiUrl { get; }

        string PortalApiUrl { get; }

        string CryptoPassword { get; }

        string CryptoIV { get; }

        string CryptoSalt { get; }

        string SPName { get; }

        string SentryDsn { get; }

        string Environment { get; }

        int RedisDatabase { get; }

        long RedisExpired { get; }

        string JWTPassword { get; }

        string JWT { get; }

        string JWTIssue { get; }

        string JWTAudience { get; }

        bool IsEnableLogstash { get; }

        string LogstashTcpHostname { get; }

        int LogstashTcpPort { get; }

        List<string> UnhandledFingerprintAttributeValues { get; }

        List<string> LogstashIgnoreSubscribers { get; }

        bool IsExtraSub { get; }

        int MaxMonthsGetAssociationByDevice { get; }
        
        long EverTaggingExpireSecond { get; }

        int EverTaggingDBIndex { get; }

        long ApiEverTaggingCacheMaxAge { get; }

        bool IsDisableJsChallenge { get; }

        bool IsVaultEnable { get; }

        IConfiguration Configuration { get; }

        T GetValue<T>(string key);

        T GetSection<T>(string key);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public const string DeviceIdEncryptKey = "&％#@?,:*!~^.";

        private readonly string cdnApiUrl;
        private readonly string portalApiUrl;
        private readonly string apiUrl;
        private readonly string spName;
        private readonly string cryptoPassword;
        private readonly string sentryDsn;
        private readonly string environment;

        private readonly string cryptoIV;
        private readonly string cryptoSalt;

        private readonly long redisExpired;
        private readonly int redisDatabase;

        private readonly string jWTPassword;

        private readonly string jWTIssue;

        private readonly string jWTAudience;

        private readonly string jWT;

        private readonly bool isEnableLogstash;

        private readonly string logstashTcpHostname;

        private readonly int logstashTcpPort;

        private readonly List<string> unhandledFingerprintAttributeValues;

        private readonly List<string> logstashIgnoreSubscribers;

        private readonly bool isExtraSub;
        
        private readonly int maxMonthsGetAssociationByDevice;
        
        private readonly long everTaggingExpireSecond;
        
        private readonly int everTaggingDBIndex;
        
        private readonly long apiEverTaggingCacheMaxAge;

        private readonly bool isDisableJsChallenge;

        private readonly bool isVaultEnable;

#pragma warning disable S1541 // Constructor complexity is acceptable for configuration initialization
        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            // API Settings
            apiUrl = GetValue<string>("AppConfig:ApiUrl") ?? string.Empty;
            cdnApiUrl = GetValue<string>("AppConfig:CDNApiUrl") ?? string.Empty;
            portalApiUrl = GetValue<string>("AppConfig:PortalApiUrl") ?? string.Empty;
            spName = GetValue<string>("StoredProcedurePath") ?? "App_Data";

            // Crypto Settings
            cryptoPassword = GetValue<string>("AppConfig:Crypto:Password") ?? string.Empty;
            cryptoIV = GetValue<string>("AppConfig:Crypto:IV") ?? string.Empty;
            cryptoSalt = GetValue<string>("AppConfig:Crypto:Salt") ?? string.Empty;

            // Storage Settings
            sentryDsn = GetValue<string>("Sentry:Dsn") ?? string.Empty;
            environment = GetValue<string>("AppConfig:Environment") ?? string.Empty;
            redisDatabase = GetIntValue("Redis:Database", 0);
            redisExpired = GetIntValue("Redis:Expired", 3600);
            everTaggingExpireSecond = GetLongValue("Redis:EverTaggingExpireSecond", 3600);
            everTaggingDBIndex = GetIntValue("Redis:EverTaggingDbIndex", 0);

            // JWT Settings
            jWTPassword = GetValue<string>("AppConfig:JWT:Password") ?? "!QAZxdr567890";
            jWTIssue = GetValue<string>("AppConfig:JWT:Issue") ?? string.Empty;
            jWTAudience = GetValue<string>("AppConfig:JWT:Audience") ?? string.Empty;
            jWT = GetValue<string>("AppConfig:JWT") ?? string.Empty;

            // Logstash Settings
            isEnableLogstash = GetBoolValue("Logstash:IsEnable");
            logstashTcpHostname = GetValue<string>("Logstash:Tcp:Hostname") ?? string.Empty;
            logstashTcpPort = GetIntValue("Logstash:Tcp:Port", 32705);
            logstashIgnoreSubscribers = GetSection<List<string>>("Logstash:IgnoreSubscribers") ?? new List<string>();

            // Fingerprint Settings
            unhandledFingerprintAttributeValues = GetSection<List<string>>("Fingerprint:UnhandledAttributeValues") ?? new List<string>();
            maxMonthsGetAssociationByDevice = GetIntValue("API:MaxMonthsGetAssociationByDevice", 0);
            apiEverTaggingCacheMaxAge = GetLongValue("ApiEverTaggingCacheMaxAge", 86400);

            // Consumer Settings
            isExtraSub = GetBoolValue("Consumer:IsExtraSub");
            isDisableJsChallenge = GetBoolValue("Consumer:IsDisableJsChallenge");
            isVaultEnable = GetBoolValue("VaultOptions:Enable");
        }

        private int GetIntValue(string key, int defaultValue)
        {
            var value = GetValue<string>(key);
            return string.IsNullOrEmpty(value) ? defaultValue : Convert.ToInt32(value);
        }

        private long GetLongValue(string key, long defaultValue)
        {
            var value = GetValue<string>(key);
            return string.IsNullOrEmpty(value) ? defaultValue : Convert.ToInt64(value);
        }

        private bool GetBoolValue(string key)
        {
            var value = GetValue<string>(key);
            return !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
        }

        public string ApiUrl => apiUrl;

        public string CryptoPassword => cryptoPassword;

        public string SPName => spName;

        public string SentryDsn => sentryDsn;

        public string Environment => environment;

        public string CryptoIV => cryptoIV;

        public string CryptoSalt => cryptoSalt;

        public string CDNApiUrl => cdnApiUrl;

        public string PortalApiUrl => portalApiUrl;

        public int RedisDatabase => redisDatabase;

        public long RedisExpired => redisExpired;

        public string JWTPassword => jWTPassword;

        public string JWT => jWT;

        public bool IsEnableLogstash => isEnableLogstash;

        public string LogstashTcpHostname => logstashTcpHostname;

        public int LogstashTcpPort => logstashTcpPort;

        public List<string> UnhandledFingerprintAttributeValues => unhandledFingerprintAttributeValues;

        public List<string> LogstashIgnoreSubscribers => logstashIgnoreSubscribers;

        public bool IsExtraSub => isExtraSub;

        public int MaxMonthsGetAssociationByDevice => maxMonthsGetAssociationByDevice;
        
        public long EverTaggingExpireSecond => everTaggingExpireSecond;

        public int EverTaggingDBIndex => everTaggingDBIndex;

        public long ApiEverTaggingCacheMaxAge => apiEverTaggingCacheMaxAge;

        public bool IsDisableJsChallenge => isDisableJsChallenge;

        public bool IsVaultEnable => isVaultEnable;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }

        public IConfiguration Configuration => configuration;

        public string JWTIssue => jWTIssue;

        public string JWTAudience => jWTAudience;
    }
}
