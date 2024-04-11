using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
	private Animator animator;
	public Button FirstButton;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		FirstButtonOn();
	}

	public void FirstButtonOn()
	{
		FirstButton.Select();
	}

	public void Restart() //재시작
	{
		Debug.Log("재시작");
	}
	public void Setting() //설정
	{
		Debug.Log("설정");
	}
	public void GoMenu()
	{
		SceneManager.LoadScene(0);
		Time.timeScale = 1;
	}

	public void Close()
	{
		animator.SetBool("Open", false);
	}

	public void PauseOff()
	{
		UIManager.Instance.IsPause = false;
		gameObject.SetActive(false);
	}

	public void PauseOn()
	{
		UIManager.Instance.IsPause = true;
	}
}
