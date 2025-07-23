using System;
using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    class DiagnosticsFactory : IDiagnosticsFactory
    {
        public IReadOnlyDictionary<string, string> CommonTags { get; } = new Dictionary<string, string>();

        public IDiagnostics Create(string packageName)
        {
            return new Diagnostics();
        }
    }
}
