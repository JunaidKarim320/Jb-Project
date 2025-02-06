using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;
    float count;
    //public Text thisText;

    const string textFormat = "{0} FPS";
    int ignoredFrames;
    float previousTime;
    float deltaTime;
    private void Awake()
    {
        //StartCoroutine(Starta());

    }

    IEnumerator Starta()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                count = (1 / Time.deltaTime);
                label.text = "FPS :" + (Mathf.Round(count));
            }
            else
            {
             //   label.text = ScriptLocalization.Pause;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    void Update()
    {
        ignoredFrames++;

        deltaTime = Time.realtimeSinceStartup - previousTime;
        if (deltaTime < 0.5f)
        {
            return;
        }

        label.text = string.Format(textFormat, (int)(ignoredFrames / deltaTime));
        ignoredFrames = 0;
        previousTime = Time.realtimeSinceStartup;
    }
    void OnGUI()
    {
        //	GUI.Label(new Rect(5, 40, 200, 50), label);
    }
}
