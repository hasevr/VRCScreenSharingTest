using UdonSharp;
using UnityEngine.UI;
using UnityEngine;
using VRC.SDKBase;

public class SSController : SSControllerBase
{
    public UdonSharp.Video.USharpVideoPlayer videoPlayer;
    override protected void DoPlayVideo()
    {
        videoPlayer.SetToAVProPlayer();
        UdonSharp.Video.VideoPlayerManager manager = videoPlayer.GetVideoManager();
        //text.text = "Got manager";
        manager.Stop();
        manager.SetToStreamPlayerMode();
        //text.text = "PlayerMode";
        manager.avProPlayer.LoadURL(getUrl());
        //text.text = "Loaded" + getUrl();

        manager.avProPlayer.Play();

        //text.text = "Playing " + getUrl();
    }
    override protected void DoStopVideo() {
        UdonSharp.Video.VideoPlayerManager manager = videoPlayer.GetVideoManager();
        manager.Stop();
    }
}
