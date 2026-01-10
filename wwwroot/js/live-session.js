const client = AgoraRTC.createClient({ mode: "rtc", codec: "vp8" });
let localTrack;

async function joinSession() {
    const appId = "YOUR_AGORA_APP_ID";
    const token = null;
    const channel = "testChannel";

    const uid = await client.join(appId, channel, token, null);

    localTrack = await AgoraRTC.createScreenVideoTrack();
    localTrack.play("screen-share");

    await client.publish([localTrack]);
    console.log("انضم الطالب وبدأ مشاركة الشاشة");
}

document.addEventListener("DOMContentLoaded", () => {
    joinSession();
});
