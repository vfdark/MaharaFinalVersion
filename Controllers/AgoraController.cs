using Microsoft.AspNetCore.Mvc;
using System;
using MaharaFinalVersion.Agora; // <- for RtcTokenBuilder

namespace MaharaFinalVersion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgoraController : ControllerBase
    {
        private readonly string AppId = "d0d5abfd738c42a2a956b631679d1971";
        private readonly string AppCertificate = "77d2043fd6c84ca48608c71b2c57fca5";

        [HttpGet("token")]
        public IActionResult GetToken([FromQuery] string channelName, [FromQuery] string uid = "0")
        {
            try
            {
                // Token valid for 1 hour
                int expireTimeInSeconds = 3600;
                uint userId = Convert.ToUInt32(uid);

                string token = RtcTokenBuilder.BuildTokenWithUid(
                    AppId,
                    AppCertificate,
                    channelName,
                    userId,
                    RtcTokenBuilder.Role.RolePublisher, // or RoleSubscriber
                    expireTimeInSeconds
                );

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
