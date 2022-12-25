using UdonSharp;
using UnityEngine.UI;
using VRC.SDKBase;


public class SSController : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)] VRCUrl urlForPC;
    [UdonSynced(UdonSyncMode.None)] VRCUrl urlForQuest;

    string playingUrl ="";

    public UdonSharp.Video.USharpVideoPlayer videoPlayer;
    public VRC.SDK3.Components.VRCUrlInputField ifUrlForPC;
    public VRC.SDK3.Components.VRCUrlInputField ifUrlForQuest;
    public Text urlTextForQuest;
    public Text text;

    void Start()
    {
        SendCustomEventDelayedSeconds("InitialPlay", 3);
        urlForPC = ifUrlForPC.GetUrl();
        urlForQuest = ifUrlForQuest.GetUrl();
    }
    public void InitialPlay()
    {
        if (playingUrl.ToString() == "" && Playable())
        {
            Play();
            playingUrl = getUrl().ToString();
        }
    }
    private void Update()
    {
        string url = ifUrlForPC.GetUrl().ToString();
        if (url.Length > 5)
        {
            string quest = "rtsp" + url.Substring(5);
            urlTextForQuest.text = quest;
        }
    }


    public void Pressed()
        {
        text.text = "Pressed";
        GetOwner();
        text.text = "Got Owner";
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
        if (isUrlUpdated()){
            playingUrl = getUrl().ToString();
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
        manager.avProPlayer.LoadURL(getUrl());
        text.text = "Loaded" + getUrl();
        manager.avProPlayer.Play();
        text.text = "Playing " + getUrl();
    }
    private VRCUrl getUrl()
    {
#if UNITY_ANDROID
        return urlForQuest;
#else
        return urlForPC;
#endif
    }
    private bool isUrlUpdated()
    {
        if (getUrl().ToString() != "" && getUrl().ToString() != playingUrl) { return true; }
        return false;
    }
    private bool Playable()
    {
        if (urlForPC.ToString() != "")
        {
            return true;
        }
        return false;
    }
}

