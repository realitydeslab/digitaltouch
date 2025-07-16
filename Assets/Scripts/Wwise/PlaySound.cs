using UnityEngine;
using UnityEngine.UI; // For UI Button
using AK.Wwise;
using System.Collections.Generic; // For Wwise Events

public class PlaySound : MonoBehaviour
{
    public AK.Wwise.Event playSineEvent;

    public List<Button> triggerButtons;

    void Start()
    {
        // 检查 Event 和 Button 是否已赋值
        if (playSineEvent == null)
        {
            Debug.LogError("Wwise Event (playSineEvent) is not assigned! Please assign it in the Inspector.");
            return;
        }
        if (triggerButtons == null)
        {
            Debug.LogError("Trigger Button is not assigned! Please assign it in the Inspector.");
            return;
        }

        foreach(var button in triggerButtons)
        {
            button.onClick.AddListener(TriggerWwiseSound);
        }
    }

    public void TriggerWwiseSound()
    {
        if (playSineEvent != null)
        {
            playSineEvent.Post(gameObject);
            Debug.Log("Playing Wwise Event: " + playSineEvent.Name);
        }
    }

    void OnDestroy()
    {
        foreach(var button in triggerButtons)
        {
            button.onClick.RemoveListener(TriggerWwiseSound);
        }
    }
}