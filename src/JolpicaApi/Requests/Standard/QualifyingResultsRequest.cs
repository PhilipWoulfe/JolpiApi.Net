using JolpicaApi.Client.Attributes;
using JolpicaApi.Responses.RaceInfo;

namespace JolpicaApi.Requests.Standard
{
    /// <summary>
    /// Represents a request for qualifying results.
    /// </summary>
    public class QualifyingResultsRequest : StandardRequest<QualifyingResultsResponse>
    {
        /// <summary>
        /// Gets or sets the qualifying position.
        /// </summary>
        [UrlTerminator, UrlSegment("qualifying")]
        public override int? QualifyingPosition { get; set; }
    }
}