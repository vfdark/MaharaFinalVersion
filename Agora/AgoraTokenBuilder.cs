using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MaharaFinalVersion.Agora
{
    public enum RtcRole
    {
        PUBLISHER = 1,
        SUBSCRIBER = 2
    }

    public class AgoraTokenBuilder
    {
        public static string BuildRtcToken(
            string appId,
            string appCertificate,
            string channelName,
            uint uid,
            RtcRole role,
            int expireTimestamp)
        {
            var token = new AccessToken(appId, appCertificate, expireTimestamp);

            var rtcService = new ServiceRtc(channelName, uid);
            rtcService.AddPrivilege(
                role == RtcRole.PUBLISHER
                    ? ServiceRtc.PrivilegePublishAudio
                    : ServiceRtc.PrivilegeJoinChannel,
                expireTimestamp
            );

            if (role == RtcRole.PUBLISHER)
            {
                rtcService.AddPrivilege(ServiceRtc.PrivilegePublishVideo, expireTimestamp);
                rtcService.AddPrivilege(ServiceRtc.PrivilegePublishDataStream, expireTimestamp);
            }

            token.AddService(rtcService);
            return token.Build();
        }
    }

    internal class AccessToken
    {
        private const string Version = "007";
        private readonly string appId;
        private readonly string appCertificate;
        private readonly int expire;
        private readonly Dictionary<ushort, IService> services = new();

        public AccessToken(string appId, string appCertificate, int expire)
        {
            this.appId = appId;
            this.appCertificate = appCertificate;
            this.expire = expire;
        }

        public void AddService(IService service)
        {
            services[service.GetServiceType()] = service;
        }

        public string Build()
        {
            var issueTs = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var salt = new Random().Next();
            var signing = BuildSign(issueTs, salt);
            var content = BuildContent(issueTs, salt);
            return Version + appId + Convert.ToBase64String(Pack(signing, content));
        }

        private byte[] BuildSign(int issueTs, int salt)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appCertificate));
            var msg = new List<byte>();
            msg.AddRange(BitConverter.GetBytes(issueTs));
            msg.AddRange(BitConverter.GetBytes(salt));
            msg.AddRange(BitConverter.GetBytes(expire));
            msg.AddRange(PackServices());
            return hmac.ComputeHash(msg.ToArray());
        }

        private byte[] BuildContent(int issueTs, int salt)
        {
            var content = new List<byte>();
            content.AddRange(BitConverter.GetBytes(issueTs));
            content.AddRange(BitConverter.GetBytes(salt));
            content.AddRange(BitConverter.GetBytes(expire));
            content.AddRange(PackServices());
            return content.ToArray();
        }

        private byte[] PackServices()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)services.Count));
            foreach (var service in services.Values)
                bytes.AddRange(service.Pack());
            return bytes.ToArray();
        }

        private static byte[] Pack(byte[] sign, byte[] content)
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes((ushort)sign.Length));
            buffer.AddRange(sign);
            buffer.AddRange(BitConverter.GetBytes((ushort)content.Length));
            buffer.AddRange(content);
            return buffer.ToArray();
        }
    }

    internal interface IService
    {
        ushort GetServiceType();
        byte[] Pack();
    }

    internal class ServiceRtc : IService
    {
        public const ushort ServiceType = 1;

        public const ushort PrivilegeJoinChannel = 1;
        public const ushort PrivilegePublishAudio = 2;
        public const ushort PrivilegePublishVideo = 3;
        public const ushort PrivilegePublishDataStream = 4;

        private readonly string channelName;
        private readonly uint uid;
        private readonly Dictionary<ushort, int> privileges = new();

        public ServiceRtc(string channelName, uint uid)
        {
            this.channelName = channelName;
            this.uid = uid;
        }

        public void AddPrivilege(ushort privilege, int expire)
        {
            privileges[privilege] = expire;
        }

        public ushort GetServiceType() => ServiceType;

        public byte[] Pack()
        {
            var buffer = new List<byte>();

            buffer.AddRange(BitConverter.GetBytes((ushort)channelName.Length));
            buffer.AddRange(Encoding.UTF8.GetBytes(channelName));

            buffer.AddRange(BitConverter.GetBytes(uid));

            buffer.AddRange(BitConverter.GetBytes((ushort)privileges.Count));
            foreach (var kv in privileges)
            {
                buffer.AddRange(BitConverter.GetBytes(kv.Key));
                buffer.AddRange(BitConverter.GetBytes(kv.Value));
            }

            return buffer.ToArray();
        }
    }
}
