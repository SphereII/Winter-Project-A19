using UnityEngine;
using UnityEngine.UI;
using OldMoatGames;

public class ImageWrapper : MonoBehaviour
{
    private AnimatedGifPlayer AnimatedGifPlayer;

    private string URL;
    public void Awake()
    {
        // Get the GIF player component
        AnimatedGifPlayer = GetComponent<AnimatedGifPlayer>();
        if (AnimatedGifPlayer == null)
            AnimatedGifPlayer = gameObject.AddComponent<AnimatedGifPlayer>();
        // Set the file to use. File has to be in StreamingAssets folder or a remote url (For example: http://www.example.com/example.gif).
        AnimatedGifPlayer.FileName = "AnimatedGIFPlayerExampe 3.gif";

        // Disable autoplay
        AnimatedGifPlayer.AutoPlay = false;

        // Add ready event to start play when GIF is ready to play
        AnimatedGifPlayer.OnReady += OnGifLoaded;

        // Add ready event for when loading has failed
        AnimatedGifPlayer.OnLoadError += OnGifLoadError;

    }

   
    public bool IsNewURL( string url )
    {
        if (string.IsNullOrEmpty(url))
            return false;

        if (url == AnimatedGifPlayer.FileName)
            return false;
        return true;

    }
    public void Init ( string url, string TargetComponent = "" )
    {
        if (string.IsNullOrEmpty(url))
            return;

        AnimatedGifPlayer.FileName = url;
        // Init the GIF player
        
        AnimatedGifPlayer.Init(TargetComponent);
    }

    private void OnGifLoaded()
    {
        Play();
    }

    private void OnGifLoadError()
    {
        Debug.Log("Error Loading GIF");
    }

    public void Play()
    {
        // Start playing the GIF
        AnimatedGifPlayer.Play();
    }

    public void Pause()
    {
        // Stop playing the GIF
        AnimatedGifPlayer.Pause();
    }

    public void OnDisable()
    {
        AnimatedGifPlayer.OnReady -= Play;
    }
}
