﻿using Newtonsoft.Json;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.MasterData
{
    internal class MapAreaJson : IRawMapArea
    {
        [JsonProperty("api_id")]
        public int Id { get; set; }
        [JsonProperty("api_name")]
        public string Name { get; set; }
        [JsonProperty("api_type")]
        public bool IsEvent { get; set; }
    }
}
