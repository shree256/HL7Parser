using System.Security.Cryptography;
using NHapi.Model.V23.Message;
using NHapi.Model.V23.Segment;

namespace HL7Parser;

public class HL7Message
{
    public string? Message { get; set; }
}

public class ParsedADTA01Model
{
    public MSH MSHSegment { get; set; }
    public PID PIDSegment { get; set; }
}

public class ADTA01MessageHeader
{
    public string App { get; set; }
    public string Facility { get; set; }
    public string MsgTime { get; set; }
    public string ControlId { get; set; }
    public string Type { get; set; }
    public string Version { get; set; }
}

public class ADTA01ParsedMessage
{
    public ADTA01MessageHeader MessageHeader { get; set; }
}