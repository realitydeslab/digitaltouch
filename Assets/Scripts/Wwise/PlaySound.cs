using UnityEngine;
using UnityEngine.UI; // For UI Button
using AK.Wwise;
using System.Collections.Generic; // For Wwise Events

public class PlaySound : MonoBehaviour
{
    public AK.Wwise.Event playEvent;

    public List<Button> triggerButtons;

    private uint playingId;


    void Start()
    {
        if (playEvent == null)
        {
            Debug.LogError("Wwise Event (playSineEvent) is not assigned! Please assign it in the Inspector.");
            return;
        }
        if (triggerButtons == null)
        {
            Debug.LogError("Trigger Button is not assigned! Please assign it in the Inspector.");
            return;
        }

        foreach (var button in triggerButtons)
        {
            button.onClick.AddListener(TriggerWwiseSound);
        }
    }

    public void TriggerWwiseSound()
    {
        if (playEvent != null)
        {
            playingId = playEvent.Post(gameObject);
            Debug.Log("Playing Wwise Event: " + playEvent.Name);
        }
    }
    public void StopWwiseSound()
    {
        if (playingId != AkUnitySoundEngine.AK_INVALID_PLAYING_ID) // Check if there's a valid playing ID
        {
            AkUnitySoundEngine.StopPlayingID(playingId);
            Debug.Log("Stopped Wwise Event with Playing ID: " + playingId);
            playingId = AkUnitySoundEngine.AK_INVALID_PLAYING_ID; // Reset the ID after stopping
        }
        else
        {
            Debug.Log("No active sound to stop or already stopped.");
        }
    }



    void OnDestroy()
    {
        foreach (var button in triggerButtons)
        {
            button.onClick.RemoveListener(TriggerWwiseSound);
        }
    }
}