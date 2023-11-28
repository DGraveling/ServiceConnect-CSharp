﻿using System.Collections.Generic;

namespace ServiceConnect.Telemetry;

public class ConsumeEventArgs
{
    public byte[] Message { get; init; }

    public string Type { get; init; } = string.Empty;

    public IDictionary<string, object> Headers { get; init; }
}