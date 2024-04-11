using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
	public Button FirstButton;

	private void OnEnable()
	{
		FirstButton.Select();
		UIManager.Instance.IsExit = true;
	}
}
