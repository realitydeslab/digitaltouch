using UnityEngine;

public class VFX_UIContorl : MonoBehaviour
{
    public GameObject Flame;
    private bool isFlameEnable = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        if (Flame != null)
        {
            Flame.SetActive(isFlameEnable);
        }
    }

    public void ToggleFlame()
    {
        isFlameEnable = !isFlameEnable;
        Flame.SetActive(isFlameEnable);
    }
}
