using UnityEngine;

public class TestControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetScale(float newScale)
    {
        // 将接收到的单个浮点数值同时应用到x, y, z轴上
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
