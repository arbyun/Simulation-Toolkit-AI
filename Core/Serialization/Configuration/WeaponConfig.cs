using System;
using System.Text.Json.Serialization;
using SimArena.Core.Configuration;

namespace SimArena.Core.Serialization.Configuration
{
    [Serializable]
    public class WeaponConfig
    {
        public string WeaponId { get; set; }
    
        [JsonConverter(typeof(JsonStringEnumConverter<WeaponType>))] 
        public WeaponType WeaponType { get; set; }
    
        public float Range { get; set; }
        public int Damage { get; set; }
    }
}