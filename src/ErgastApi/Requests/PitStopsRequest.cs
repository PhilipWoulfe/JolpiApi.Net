using JolpiApi.Client.Attributes;
using JolpiApi.Responses.RaceInfo;

namespace JolpiApi.Requests
{
    /// <summary>
    /// Represents a request for pit stop information.
    /// </summary>
    public class PitStopsRequest : ErgastRequest<PitStopsResponse>
    {
        /// <summary>
        /// Gets or sets the lap number for the pit stop request.
        /// </summary>
        [UrlSegment("laps")]
        public int? Lap { get; set; }

        /// <summary>
        /// Gets or sets the pit stop number for the request.
        /// </summary>
        [UrlTerminator, UrlSegment("pitstops")]
        public int? PitStop { get; set; }
    }
}