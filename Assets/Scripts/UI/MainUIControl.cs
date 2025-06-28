using UnityEngine;

public class MainUIControl : MonoBehaviour
{
    public void DoQuit()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
}
