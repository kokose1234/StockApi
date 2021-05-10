//  Copyright 2021 Jonguk Kim
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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