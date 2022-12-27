using UdonSharp;
using UnityEngine.UI;
using UnityEngine;
using VRC.SDKBase;

public class SSControllerBase: UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)] VRCUrl urlForPC;
    [UdonSynced(UdonSyncMode.None)] VRCUrl urlForQuest;
    [UdonSynced(UdonSyncMode.None)] int playing;
    private int lastPlaying;

    public Transform transformToFlip;
    private float scaleY;

    public VRC.SDK3.Components.VRCUrlInputField ifUrlForPC;
    public VRC.SDK3.Components.VRCUrlInputField ifUrlForQuest;
    public InputField ifTextForQuest;
    public Text text;

    void Start()
    {
        playing = 0;
        lastPlaying = 0;
        urlForPC = ifUrlForPC.GetUrl();
        urlForQuest = ifUrlForQuest.GetUrl();
        scaleY = transformToFlip.localScale.y;
        SendCustomEventDelayedSeconds("InitialPlay", 3);
    }
    public void InitialPlay()
    {
        if (IsPlayingUpdated() && playing != 0)
        {
            Play();
        }
    }
    private void Update()
    {
        string url = ifUrlForPC.GetUrl().ToString();
        if (url.Length > 5)
        {
            string quest = "rtsp" + url.Substring(5);
            ifTextForQuest.text = quest;
        }
    }

    public void OnClickPlay()
    {
        text.text = "OnClickPlay()";

        urlForPC = ifUrlForPC.GetUrl();
        urlForQuest = ifUrlForQuest.GetUrl();
        playing++;
        Play();

        TakeOwner();
        RequestSerialization();
        text.text = "OnClickPlay()";
    }
    public void OnClickReload()
    {
        if(getUrl().ToString() != "")
        {
            DoPlayVideo();
        }
    }
    public void OnClickStop()
    {
        playing = 0;
        Stop();
        TakeOwner();
        RequestSerialization();
        text.text = "OnClickStop()";
    }
    public bool AmIOwner()
    {
        return Networking.IsOwner(Networking.LocalPlayer, this.gameObject);
    }
    public void TakeOwner()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
    }
    public override void OnDeserialization()
    {
        if (IsPlayingUpdated() && !AmIOwner()) {
            text.text = "OnDeserialization()playing:" + playing + " last:" + lastPlaying;
            if (playing != 0)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }
    private void Play()
    {
        lastPlaying = playing;
        UpsideDown();
        DoPlayVideo();
    }
    private void Stop()
    {
        lastPlaying = playing = 0;
        DoStopVideo();
        UpsideUp();
    }
    protected virtual void DoPlayVideo() { }
    protected virtual void DoStopVideo() { }

    private void UpsideDown()
    {
#if UNITY_ANDROID
        Vector3 scale = transformToFlip.localScale;
        if (scale.y * scaleY > 0) {
            scale.y *= -1;
            transformToFlip.localScale = scale;
        }
#endif
    }
    private void UpsideUp()
    {
        Vector3 scale = transformToFlip.localScale;
        if (scale.y * scaleY < 0)
        {
            scale.y *= -1;
            transformToFlip.localScale = scale;
        }
    }
    protected VRCUrl getUrl()
    {
#if UNITY_ANDROID
        return urlForQuest;
#else
        return urlForPC;
#endif
    }
    private bool IsPlayingUpdated()
    {
        if (lastPlaying != playing) { return true; }
        return false;
    }
}

