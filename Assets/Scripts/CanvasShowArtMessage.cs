using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasShowArtMessage : MonoBehaviour
{
    public GameObject[] artObjects;
    public AudioSource audioSource;
    public UnityEngine.Video.VideoPlayer videoPlayer;
    private bool isArtShowing;

    private string currentArtName;

    void Start()
    {
        HideAll();
    } 

    void Update()
    {

    }

    void HideAll()
    {
        foreach (GameObject obj in artObjects)
        {
            obj.SetActive(false);
        }
        isArtShowing = false;
    }

    public void ShowArtByName(string objName)
    {
        if (isArtShowing) {return;}
        
        HideAll();
        foreach (GameObject obj in artObjects)
        {
            if (obj.name == objName)
            {
                obj.SetActive(true);
                isArtShowing = true;
                currentArtName = objName;
                if(obj.name == "WorldVideoCanvasArt")
                {
                    ToggleAudio();
                    ToggleVideoVolume();
                }
            }
        }
    }

    public void CloseArt()
    {
        HideAll();
        if (currentArtName == "WorldVideoCanvasArt")
        {
            ToggleAudio();
            ToggleVideoVolume();
        }
        currentArtName = "";
    }

    public void ToggleAudio()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

    public void ToggleVideoVolume()
    {
        if (videoPlayer == null) return;

        float currentVolume = videoPlayer.GetDirectAudioVolume(0);

        if (currentVolume > 0f)
        {
            videoPlayer.SetDirectAudioVolume(0, 0f);
        }
        else
        {
            videoPlayer.SetDirectAudioVolume(0, 1f);
        }
    }
}
