using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    class Metrics : IMetrics
    {
        internal IDictionary<string, string> PackageTags { get; } = new Dictionary<string, string>();

        void IMetrics.SendGaugeMetric(string name, double value, IDictionary<string, string> tags)
        {
            // do nothing
        }

        void IMetrics.SendHistogramMetric(string name, double time, IDictionary<string, string> tags)
        {
            // do nothing
        }

        void IMetrics.SendSumMetric(string name, double value, IDictionary<string, string> tags)
        {
            // do nothing
        }
    }
}
