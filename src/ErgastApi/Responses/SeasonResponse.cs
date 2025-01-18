﻿using System.Collections.Generic;
using JolpiApi.Responses.Models;
using JolpiApi.Serialization;

namespace JolpiApi.Responses
{
    public class SeasonResponse : ErgastResponse
    {
        [JsonPathProperty("SeasonTable.Seasons")]
        public IList<Season> Seasons { get; private set; }
    }
}
