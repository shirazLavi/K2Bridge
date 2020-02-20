// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace K2Bridge.Models.Request.Queries
{
    using K2Bridge.JsonConverters;
    using K2Bridge.Models.Request;
    using K2Bridge.Visitors;
    using Newtonsoft.Json;

    [JsonConverter(typeof(RangeClauseConverter))]
    internal class RangeClause : KustoQLBase, ILeafClause, IVisitable
    {
        public string FieldName { get; set; }

#nullable enable
        public object? GTEValue { get; set; }

        // isn't created by kibana but kept here for completeness
        public object? GTValue { get; set; }

        public object? LTEValue { get; set; }

        public object? LTValue { get; set; }
#nullable disable

        public string Format { get; set; }

        /// <inheritdoc/>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
