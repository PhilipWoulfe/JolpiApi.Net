﻿using System.Collections.Generic;
using ErgastApi.Serialization;

namespace ErgastApi.Responses
{
    public class CircuitResponse : ErgastResponse
    {
        [JsonPathProperty("CircuitTable.Circuits")]
        public IList<Circuit> Circuits { get; set; }
    }
}
