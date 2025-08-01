using System;
using System.Collections.Generic;

namespace Unity.Services.Core.Telemetry.Internal
{
    class Diagnostics : IDiagnostics
    {
        internal IDictionary<string, string> PackageTags { get; } = new Dictionary<string, string>();

        public void SendDiagnostic(string name, string message, IDictionary<string, string> tags = null)
        {
            // do nothing
        }
    }
}
