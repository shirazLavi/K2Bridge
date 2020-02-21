// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace K2Bridge.Visitors
{
    using System;
    using K2Bridge.Models.Request.Queries;
    using Newtonsoft.Json.Linq;

    /// <content>
    /// A visitor for the <see cref="RangeClause"/> element.
    /// </content>
    internal partial class ElasticSearchDSLVisitor : IVisitor
    {
        /// <inheritdoc/>
        public void Visit(RangeClause rangeClause)
        {
            Ensure.IsNotNull(rangeClause, nameof(rangeClause));
            EnsureClause.StringIsNotNullOrEmpty(rangeClause.FieldName, nameof(rangeClause.FieldName));
            EnsureClause.IsNotNull(rangeClause.GTEValue, nameof(rangeClause.GTEValue));

            // format used by Kibana 6
            if (rangeClause.Format == "epoch_millis")
            {
                // default time filter through a rangeClause query uses epoch times with GTE+LTE
                EnsureClause.IsNotNull(rangeClause.LTEValue, nameof(rangeClause.LTEValue));

                rangeClause.KustoQL = $"{rangeClause.FieldName} >= fromUnixTimeMilli({rangeClause.GTEValue}) {KustoQLOperators.And} {rangeClause.FieldName} <= fromUnixTimeMilli({rangeClause.LTEValue})";
            }

            // format used by Kibana 7
            else if (rangeClause.Format == "strict_date_optional_time")
            {
                // default time filter through a rangeClause query uses epoch times with GTE+LTE
                EnsureClause.IsNotNull(rangeClause.LTEValue, nameof(rangeClause.LTEValue));

                var gte = ((JValue)rangeClause.GTEValue).Value<DateTime>().ToString("o");
                var lte = ((JValue)rangeClause.LTEValue).Value<DateTime>().ToString("o");
                rangeClause.KustoQL = $"{rangeClause.FieldName} >= datetime(\"{gte}\") {KustoQLOperators.And} {rangeClause.FieldName} <= datetime(\"{lte}\")";
            }
            else
            {
                // general "is between" filter on numeric fields uses a rangeClause query with GTE+LT (not LTE like above)
                EnsureClause.IsNotNull(rangeClause.LTValue, nameof(rangeClause.LTValue));

                rangeClause.KustoQL = $"{rangeClause.FieldName} >= {rangeClause.GTEValue} and {rangeClause.FieldName} < {rangeClause.LTValue}";
            }
        }
    }
}
