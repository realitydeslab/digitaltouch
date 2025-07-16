using UnityEngine;
using UnityEngine.UI;
using AK.Wwise;

public class RTPCController : MonoBehaviour
{
    public AK.Wwise.RTPC myRTPC;

    // 引用 UI 滑块
    public Slider volumeSlider; // for testing in editor.
    public float RPTC_Value;

    void Start()
    {
        // 检查 Game Parameter 和 Slider 是否已赋值
        if (myRTPC == null)
        {
            Debug.LogError("Wwise Game Parameter is not assigned!");
            return;
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnRPTCValueChanged);
            SetRTPCValue(volumeSlider.value);
        }
    }

    private void OnRPTCValueChanged(float value)
    {
        SetRTPCValue(value);
    }

    public void SetRTPCValue(float value)
    {
        RPTC_Value = value;
        AkUnitySoundEngine.SetRTPCValue(myRTPC.Name, value, gameObject);
    }

    void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnRPTCValueChanged);
        }
    }
}