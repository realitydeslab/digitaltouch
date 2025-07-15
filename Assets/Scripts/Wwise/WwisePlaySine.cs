using UnityEngine;
using UnityEngine.UI; // For UI Button
using AK.Wwise; // For Wwise Events

public class WwisePlaySine : MonoBehaviour
{
    // 在 Inspector 中拖拽你的 Wwise Event 到这里
    public AK.Wwise.Event playSineEvent;

    // 引用 UI 按钮
    public Button triggerButton;

    void Start()
    {
        // 检查 Event 和 Button 是否已赋值
        if (playSineEvent == null)
        {
            Debug.LogError("Wwise Event (playSineEvent) is not assigned! Please assign it in the Inspector.");
            return;
        }
        if (triggerButton == null)
        {
            Debug.LogError("Trigger Button is not assigned! Please assign it in the Inspector.");
            return;
        }

        // 为按钮添加点击事件监听器
        triggerButton.onClick.AddListener(TriggerWwiseSound);

        // AkBank 组件会在 Start 时自动加载 SoundBank，这里不需要额外代码加载
        Debug.Log("Wwise SoundBank should be loaded by AkBank component.");
    }

    // 当按钮被点击时调用的方法
    public void TriggerWwiseSound()
    {
        if (playSineEvent != null)
        {
            // 播放 Wwise Event。将声音发布到此 GameObject 上。
            // 对于UI声音，通常不需要3D定位，但指定一个GameObject是常见做法。
            playSineEvent.Post(gameObject);
            Debug.Log("Playing Wwise Event: " + playSineEvent.Name);
        }
    }

    void OnDestroy()
    {
        // 在对象销毁时移除监听器，防止内存泄漏
        if (triggerButton != null)
        {
            triggerButton.onClick.RemoveListener(TriggerWwiseSound);
        }
        // AkBank 组件会在 OnDestroy 时自动卸载 SoundBank，这里不需要额外代码卸载
        Debug.Log("Wwise SoundBank should be unloaded by AkBank component.");
    }
}