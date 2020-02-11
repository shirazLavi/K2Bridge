// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace K2Bridge.Tests.End2End
{
    using FluentAssertions.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    /// <summary>
    /// Parallel end-to-end tests loading data into Kusto and Elasticsearch and assuring
    /// that K2Bridge returns equivalent outputs to Elasticsearch.
    /// </summary>
    [TestFixture]
    public class ParallelApiTest : KustoTestBase
    {
        /// <summary>
        /// Ensure the JSON response at the API root (containing general cluster information)
        /// is equivalent.
        /// This is not actually required for K2Bridge functionality,
        /// but is a test for the generic passthrough functionality
        /// to the K2Bridge internal Elasticsearch.
        /// </summary>
        [Test]
        [Description("Cluster general info (at API root URL)")]
        public void ClusterInfo_Equivalent()
        {
            var es = ESClient().ClusterInfo();
            var k2 = K2Client().ClusterInfo();
            AssertJsonIdentical(k2.Result, es.Result);
        }

        [Test]
        [Description("/_msearch Kibana aggregation query returning zero results")]
        public void MSearch_ZeroResults_Equivalent()
        {
            ParallelQuery($"{FLIGHTSDIR}/MSearch_ZeroResults_Equivalent.json", minResults: 0);
        }

        [Test]
        [Description("/_msearch Kibana aggregation query returning two results")]
        public void MSearch_TwoResults_Equivalent()
        {
            ParallelQuery($"{FLIGHTSDIR}/MSearch_TwoResults_Equivalent.json");
        }

        [Test]
        [Description("/_msearch Kibana aggregation query with text filter")]
        public void MSearch_TextFilter_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_TextFilter_Equivalent.json");
        }

        [Test]
        [Description("/_msearch Kibana query with text substring")]
        public void MSearch_TextContains_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_Text_Contains.json");
        }

        [Test]
        [Description("/_msearch Kibana query with quotation text substring")]
        public void MSearch_Quotation_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_Quotation.json");
        }

        [Test]
        [Description("/_msearch Kibana query with numeric field")]
        public void MSearch_Numeric_Equivalent_WithoutHighlight()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_Numeric.json", validateHighlight: false);
        }

        // TODO: when bug https://dev.azure.com/csedevil/K2-bridge-internal/_workitems/edit/1695 is fixed merge this test with
        // MSearch_Numeric_Equivalent_WithoutHighlight and remove "validateHighlight: false" from it.
        [Test]
        [Description("/_msearch Kibana query with numeric field")]
        [Ignore("Bug#1695")]
        public void MSearch_Numeric_Equivalent_WithHighlight()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_Numeric.json");
        }

        [Test]
        [Description("/_msearch Kibana query with text multiple words substring")]
        public void MSearch_TextContainsMultiple_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_Text_Contains_Multiple.json");
        }

        [Test]
        [Description("/_msearch Kibana aggregation query with text (includes prefix) filter")]
        public void MSearch_TextFilter_Prefix_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_TextFilter_Prefix_Equivalent.json");
        }

        [Test]
        [Description("/_msearch Kibana aggregation query with text (includes wildcard) filter")]
        public void MSearch_TextFilter_Wildcard_Equivalent()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_TextFilter_Wildcard_Equivalent.json");
        }

        [Test]
        [Description("/_msearch Kibana aggregation query with text (includes specific field) filter")]
        public void MSearch_TextFilter_FieldSpecific_Equivalent_WithoutHighlight()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_TextFilter_FieldSpecific_Equivalent.json", validateHighlight: false);
        }

        // TODO: when bug https://dev.azure.com/csedevil/K2-bridge-internal/_workitems/edit/1681 is fixed merge this test with
        // MSearch_TextFilter_FieldSpecific_Equivalent_WithoutHighlight and remove "validateHighlight: false" from it.
        [Test]
        [Description("/_msearch Kibana aggregation query with text (includes specific field) filter")]
        [Ignore("Bug#1681")]
        public void MSearch_TextFilter_FieldSpecific_Equivalent_WithHighlight()
        {
            ParallelQuery(
                $"{FLIGHTSDIR}/MSearch_TextFilter_FieldSpecific_Equivalent.json", validateHighlight: false);
        }

        [Test]
        [Description("/_msearch Kibana aggregation query with multiple filters")]

        // TODO: fix timezone in bucketing and enable test
        // https://dev.azure.com/csedevil/K2-bridge-internal/_workitems/edit/1479
        // TODO: fix multiple highlights and enable test
        // https://dev.azure.com/csedevil/K2-bridge-internal/_workitems/edit/1681
        // TODO: fix numeric highlights and enable test
        // https://dev.azure.com/csedevil/K2-bridge-internal/_workitems/edit/1695
        [Ignore("Requires fixing issues 1479 and 1681 and 1695")]
        public void MSearch_MultipleFilters_Equivalent()
        {
            ParallelQuery($"{FLIGHTSDIR}/MSearch_MultipleFilters_Equivalent.json");
        }

        // NB Timestamp sorting is already covered by other test cases
        [Test]
        [TestCase("MSearch_Sort_String.json")]
        [TestCase("MSearch_Sort_Double.json")]
        [Description("/_msearch sort attribute with various data types")]
        public void MSearch_Sort_Equivalent(string queryFileName)
        {
            ParallelQuery($"{FLIGHTSDIR}/{queryFileName}");
        }

        [Test]
        [Description("/_search index list Kibana query")]
        public void Search_Equivalent()
        {
            var es = ESClient().Search();
            var k2 = K2Client().Search($"{KustoDatabase()}:{INDEX}");
            AssertJsonIdentical(k2.Result, es.Result);
        }

        [Test]
        [Description("/{index}/_field_caps field capabilities Kibana query")]
        public void FieldCaps_Equivalent_WithoutDatabaseName()
        {
            var es = ESClient().FieldCaps(INDEX);
            var k2 = K2Client().FieldCaps(INDEX);
            AssertJsonIdentical(k2.Result, es.Result);
        }

        [Test]
        [Description("/{index}/_field_caps field capabilities Kibana query")]
        public void FieldCaps_Equivalent_WithDatabaseName()
        {
            var es = ESClient().FieldCaps(INDEX);
            var k2 = K2Client().FieldCaps($"{KustoDatabase()}%3A{INDEX}");
            AssertJsonIdentical(k2.Result, es.Result);
        }

        private static void AssertJsonIdentical(JToken k2, JToken es)
        {
            k2.Should().BeEquivalentTo(es);
        }

        private void ParallelQuery(string esQueryFile, string k2QueryFile = null, int minResults = 1, bool validateHighlight = true)
        {
            var es = ESClient().MSearch(INDEX, esQueryFile, validateHighlight);
            var k2 = K2Client().MSearch(INDEX, k2QueryFile ?? esQueryFile, validateHighlight);
            var t = es.Result.SelectToken("responses[0].hits.total");
            Assert.IsTrue(t.Value<int>() >= minResults);
            AssertJsonIdentical(k2.Result, es.Result);
        }
    }
}
