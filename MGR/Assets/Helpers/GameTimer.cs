using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    public float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timerText.text = FormatTime(elapsedTime);
    }

    public string FormatTime(float time) {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
