using System;
using JolpiApi.Serialization;
using JolpiApi.Serialization.Converters;
using Newtonsoft.Json;

namespace JolpiApi.Responses.Models.RaceInfo
{
    public class RaceResult : ResultBase
    {
        /// <summary>
        /// Finishing position.
        /// R = Retired, D = Disqualified, E = Excluded, W = Withdrawn, F = Failed to qualify, N = Not classified.
        /// See <see cref="StatusText"/> for more info.
        /// </summary>
        [JsonProperty("positionText")]
        public string PositionText { get; private set; }

        public bool Retired
        {
            get
            {
                if (PositionText == "R")
                    return true;

                return Status > FinishingStatusId.Disqualified && !Status.ToString().StartsWith("Laps");
            }
        }

        public bool Disqualified => PositionText == "D";

        /// <summary>
        /// Indicates if the driver was classified (not retired and finished 90% of the race).
        /// </summary>
        public bool Classified => int.TryParse(PositionText, out _);

        [JsonProperty("points")]
        public double Points { get; private set; }

        /// <summary>
        /// Grid position, i.e. starting position.
        /// A value of 0 means the driver started from the pit lane.
        /// </summary>
        [JsonProperty("grid")]
        public int Grid { get; private set; }

        public bool StartedFromPitLane => Grid == 0;

        [JsonProperty("laps")]
        public int Laps { get; private set; }

        [JsonProperty("status")]
        public string StatusText { get; private set; }

        [JsonIgnore]
        public FinishingStatusId Status => FinishingStatusIdParser.Parse(StatusText);

        /// <summary>
        /// Fastest lap info. Only included from 2004 season and onwards.
        /// </summary>
        [JsonProperty(nameof(FastestLap))]
        public FastestLap FastestLap { get; private set; }

        /// <summary>
        /// Total race time. This is null for lapped cars.
        /// </summary>
        [JsonPathProperty("Time.millis")]
        [JsonConverter(typeof(MillisecondsTimeSpanConverter))]
        public TimeSpan? TotalRaceTime { get; private set; }

        /// <summary>
        /// Gap to winner. This is null for the winner and lapped cars.
        /// </summary>
        [JsonPathProperty("Time.time")]
        [JsonConverter(typeof(StringGapTimeSpanConverter))]
        public TimeSpan? GapToWinner { get; private set; }
    }
}