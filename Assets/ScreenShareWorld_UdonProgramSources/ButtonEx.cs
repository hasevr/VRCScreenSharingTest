using UdonSharp;
using UnityEngine.UI;
using VRC.SDKBase;


public class ButtonEx : UdonSharpBehaviour
{
    public InputField channelInput;
    public VRCUrl[] urlForWindows;
    public VRCUrl[] urlForAndroid;
    public Text text;
    public UdonSharp.Video.USharpVideoPlayer videoPlayer;
    [UdonSynced(UdonSyncMode.None)] int channel = -1;
    int lastChannel = -1;
    string screenIdPrev;
    void Start()
    {
        SendCustomEventDelayedSeconds("InitialPlay", 3);
    }
    public void InitialPlay()
    {
        lastChannel = -1;
        if (channel >= 0)
        {
            Play();
            lastChannel = channel;
        }
        //text.text = "InitalPlay " + channel;
    }
    public void Pressed()
    {
        text.text = "Pressed";
        GetOwner();
        text.text = "Got Owner";
        channel = int.Parse(channelInput.text);
        RequestSerialization();
        text.text = "playing";
        videoPlayer.SetToAVProPlayer();
        text.text = "Before Play";
        Play();
    }
    public bool IsOwner()
    {
        return Networking.IsOwner(Networking.LocalPlayer, this.gameObject);
    }
    public void GetOwner()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
    }
    public override void OnDeserialization()
    {
        if (channel >= 0 && channel != lastChannel)
        {
            lastChannel = channel;
            if (!IsOwner()) Play();
        }
    }
    private void Play()
    {
        UdonSharp.Video.VideoPlayerManager manager = videoPlayer.GetVideoManager();
        text.text = "Got manager";
        manager.Stop();
        manager.SetToStreamPlayerMode();
        text.text = "PlayerMode";
#if UNITY_ANDROID
        manager.avProPlayer.LoadURL(urlForAndroid[channel]);
        text.text = "Android Load";
#else
        manager.avProPlayer.LoadURL(urlForWindows[channel]);
        text.text = "Windows Load";
#endif
        manager.avProPlayer.Play();
        text.text = "Playing " + channel.ToString();
    }
}
