using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
	private float Sec = 0f;
	private int Min = 0;

    private TextMeshProUGUI timeText;

	private void Awake()
	{
		timeText = GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update()
    {
		Timer();

	}

    void Timer()
    {
		Sec += Time.deltaTime;

		timeText.text = string.Format("{0:D2}:{1:D2}", Min, (int)Sec);

        if ((int)Sec > 59)
        {
            Sec = 0;
            Min++;
        }
	}
}
