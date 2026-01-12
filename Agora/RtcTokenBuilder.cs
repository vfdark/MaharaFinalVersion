// File: MaharaFinalVersion/Agora/RtcTokenBuilder.cs
using System;
using System.Text;
using System.Security.Cryptography;

namespace MaharaFinalVersion.Agora  // make sure this is correct
{
    public class RtcTokenBuilder
    {
        public enum Role
        {
            RoleAttendee = 0,
            RolePublisher = 1,
            RoleSubscriber = 2,
            RoleAdmin = 101
        }

        public static string BuildTokenWithUid(string appId, string appCertificate, string channelName, uint uid, Role role, int expireTimeInSeconds)
        {
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long privilegeExpire = currentTimestamp + expireTimeInSeconds;

            string message = $"{appId}{channelName}{uid}{(int)role}{privilegeExpire}";
            byte[] keyBytes = Encoding.UTF8.GetBytes(appCertificate);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
