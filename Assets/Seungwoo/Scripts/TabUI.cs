using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabUI : MonoBehaviour
{
	private Animator animator;
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Close()
	{
		animator.SetBool("Open", false);
	}

	public void TabOff()
	{
		UIManager.Instance.IsTab = false;
		gameObject.SetActive(false);
	}

	public void TabOn()
	{
		UIManager.Instance.IsTab = true;
	}
}
