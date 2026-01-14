using Microsoft.AspNetCore.Mvc;
using System;
using MaharaFinalVersion.Agora;

namespace MaharaFinalVersion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgoraController : ControllerBase
    {
        private const string AppId = "d0d5abfd738c42a2a956b631679d1971";
private const string AppCertificate = "77d2043fd6c84ca48608c71b2c57fca5";

        [HttpGet("token")]
        public IActionResult GetToken(
            [FromQuery] string channelName,
            [FromQuery] uint uid = 0,
            [FromQuery] bool isHost = false)
        {
            if (string.IsNullOrEmpty(channelName))
                return BadRequest("Channel name is required");

            try
            {
                // ✅ EXPIRE TIMESTAMP (NOT duration)
                int expireTimestamp =
                    (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600;

                var role = isHost
                    ? RtcRole.PUBLISHER
                    : RtcRole.SUBSCRIBER;

                string token = AgoraTokenBuilder.BuildRtcToken(
                    AppId,
                    AppCertificate,
                    channelName,
                    uid,
                    role,
                    expireTimestamp
                );

                Console.WriteLine(
                    $"Agora token generated | channel={channelName}, uid={uid}, role={role}");

                return Ok(new
                {
                    appId = "d0d5abfd738c42a2a956b631679d1971",
                    channel = channelName,
                    uid = uid,
                    token = token
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Agora token error: " + ex);
                return StatusCode(500, "Failed to generate Agora token");
            }
        }
    }
}
