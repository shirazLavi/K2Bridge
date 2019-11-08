﻿namespace K2Bridge.Models.Request.Queries
{
    using K2Bridge.Models.Request;
    using K2Bridge.Visitors;
    using Newtonsoft.Json;

    [JsonConverter(typeof(ExistsQueryConverter))]
    internal class ExistsQuery : KQLBase, ILeafQueryClause, IVisitable
    {
        public string FieldName { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}