using System;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHapi.Base.Parser;
using NHapi.Model.V23.Message;
using NHapi.Model.V23.Segment;
using NHapi.Model.V23.Datatype;

namespace HL7Parser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HL7ParserController : ControllerBase
{

    [HttpPost]
    public IActionResult HL7MessageConverter([FromBody] HL7Message payload)
    {
        // Sample Message Payload
        //{
        //    "message":"MSH|^~\\&|SUNS1|OVI02|AZIS|CMD|200606221348||ADT^A01|1049691900|P|2.3\rEVN|A01|200803051509||||200803031508\rPID|||5520255^^^PK^PK~ZZZZZZ83M64Z148R^^^CF^CF~ZZZZZZ83M64Z148R^^^SSN^SSN^^20070103^99991231~^^^^TEAM||ZZZ^ZZZ||19830824|F||||||||||||||||||||||N\rZPV|Some Custom Notes|Additional custom description of the visit goes here"
        //}

        var message = payload.Message;
        var parser = new PipeParser();

        try
        {
            var parsedMessage = parser.Parse(message, "2.3");
            var messageHeader = new ADTA01MessageHeader();
            var adtParsedMessage = new ADTA01ParsedMessage();
            var patient = new Patient();

            var adtMessage = parsedMessage as NHapi.Model.V23.Message.ADT_A01;
            var msh = adtMessage.MSH;
            var pid = adtMessage.PID;

            messageHeader.App = msh.SendingApplication.NamespaceID.Value;
            messageHeader.ControlId = msh.MessageControlID.Value;
            messageHeader.Facility = msh.SendingFacility.NamespaceID.Value;
            messageHeader.Type = msh.MessageType.MessageType.Value;
            messageHeader.Version = msh.VersionID.Value;
            messageHeader.MsgTime = msh.DateTimeOfMessage.TimeOfAnEvent.Value;


            var clubbed_ids = "";
            foreach (CX id in pid.GetPatientIDInternalID()) {
                clubbed_ids = clubbed_ids + id.ID.Value;
            }

            foreach (XPN name in pid.GetPatientName()) {
                var fullname = name.GivenName+", "+name.FamilyName;
                patient.Name = fullname;
            }

            patient.Id = clubbed_ids;
            patient.Dob = pid.DateOfBirth.TimeOfAnEvent.Value;
            patient.Sex = pid.Sex.Value;

            adtParsedMessage.MessageHeader = messageHeader;
            adtParsedMessage.patient = patient;

            string strJson = JsonSerializer.Serialize<ADTA01ParsedMessage>(adtParsedMessage);
            return Ok(strJson);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured -> {e.StackTrace}");
        }

        return Ok("Parse Failed");
    }
}

