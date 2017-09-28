﻿using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace toofz.NecroDancer.Web.Api.Models
{
    /// <summary>
    /// A Crypt of the NecroDancer leaderboard entry.
    /// </summary>
    [DataContract]
    public sealed class EntryDTO
    {
        /// <summary>
        /// The leaderboard that contains the entry.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DataMember(Name = "leaderboard")]
        public LeaderboardDTO Leaderboard { get; set; }
        /// <summary>
        /// The player that submitted the entry.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DataMember(Name = "player")]
        public PlayerDTO Player { get; set; }
        /// <summary>
        /// The rank of the entry on the leaderboard.
        /// </summary>
        [DataMember(Name = "rank")]
        public int Rank { get; set; }
        /// <summary>
        /// The raw score value.
        /// </summary>
        [DataMember(Name = "score")]
        public int Score { get; set; }
        /// <summary>
        /// The zone and level that the entry ends on.
        /// </summary>
        [DataMember(Name = "end")]
        public EndDTO End { get; set; }
        /// <summary>
        /// The entity that killed the player. This may be null if the replay has not been parsed yet or 
        /// there was an error parsing the replay.
        /// </summary>
        [DataMember(Name = "killed_by")]
        public string KilledBy { get; set; }
        /// <summary>
        /// Version of the entry's replay. This may be null if the replay has not been parsed yet or 
        /// there was an error parsing the replay.
        /// </summary>
        [DataMember(Name = "version")]
        public int? Version { get; set; }
    }
}