using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryUI : MonoBehaviour
{
	private Animator animator;
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
    {
    }

	public void Close()
	{
		animator.SetBool("Open", false);
	}

	public void DictOff()
	{
		UIManager.Instance.IsDict = false;
		gameObject.SetActive(false);
	}

	public void DictOn()
	{
		UIManager.Instance.IsDict = true;
	}
}