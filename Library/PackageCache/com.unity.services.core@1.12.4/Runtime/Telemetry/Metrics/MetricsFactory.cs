using System;
using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    class MetricsFactory : IMetricsFactory
    {
        public IReadOnlyDictionary<string, string> CommonTags { get; } = new Dictionary<string, string>();

        public IMetrics Create(string packageName)
        {
            return new Metrics();
        }
    }
}
