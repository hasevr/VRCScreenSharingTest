using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;


public class ButtonEx : UdonSharpBehaviour
{
    public VRCUrl urlForWindows;
    public VRCUrl urlForAndroid;
    public Text text;
    public UdonSharp.Video.USharpVideoPlayer videoPlayer;
    [UdonSynced(UdonSyncMode.None)] bool playing = false;
    string screenIdPrev;
    void Start()
    {
    }
    public void Pressed()
    {
        text.text = "Pressed";
        GetOwner();
        text.text = "Got Owner";
        playing = true;
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
        if (playing)
        {
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
        manager.avProPlayer.LoadURL(urlForAndroid);
        text.text = "Android Load";
#else
        manager.avProPlayer.LoadURL(urlForWindows);
        text.text = "Windows Load";
#endif
        manager.avProPlayer.Play();
        text.text = "Playing";
    }
}
