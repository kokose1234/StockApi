using System;
using System.IO;
using Newtonsoft.Json;

namespace StockApi.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed record Setting
    {
        private static Setting profile;
        internal static Setting Value => profile ??= Load();

        [JsonProperty("apiKey")] 
        public string ApiKey { get; init; }


        private static Setting Load()
        {
            if (!File.Exists("Setting.json"))
            {
                Environment.Exit(1);
            }

            return JsonConvert.DeserializeObject<Setting>(File.ReadAllText("Setting.json"));
        }
    }
}