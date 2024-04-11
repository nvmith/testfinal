using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	private static UIManager instance;
	public static UIManager Instance => instance;

	public GameObject PauseUI;
	public GameObject TabUI;
	public GameObject DictUI;
	public GameObject ExitUI;

	[HideInInspector]
	public bool IsPause = false;
	[HideInInspector]
	public bool IsTab = false;
	[HideInInspector]
	public bool IsDict = false;
	[HideInInspector]
	public bool IsExit = false;

	private void Awake()
	{
		Init();
	}

	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
		{
			OpenPause();
		}

		if(Input.GetKeyDown(KeyCode.Tab))
		{
			OpenTab();
		}

		if(Input.GetKeyDown(KeyCode.I))
		{
			OpenDict();
		}
	}

	private void Init()
	{
		if (Instance == null)
		{
			instance = this;
			//DontDestroyOnLoad(this.gameObject);
		}
		//else
		//{
		//	Destroy(this.gameObject);
		//}
	}

	private void OpenDict() //도감
	{
		if (!IsDict)
		{
			DictUI.SetActive(true);
			PauseTime(true);
		}
		else
		{
			PauseTime(false);
			DictUI.GetComponent<DictionaryUI>().Close();
		}
	}

	private void OpenTab() //탭
	{
		if (!IsTab)
		{
			TabUI.SetActive(true);
		}
		else
		{
			TabUI.GetComponent<TabUI>().Close();
		}
	}

	private void OpenPause() //일시정지
	{
		if(!IsPause)
		{
			PauseTime(true);
			PauseUI.SetActive(true);
		}
		else
		{
			if(IsExit)
			{
				ExitUI.SetActive(false);
			}
			else
			{
				PauseTime(false);
				PauseUI.GetComponent<PauseUI>().Close();
			}
		}
	}

	private void PauseTime(bool IsPause)
	{
		if(IsPause)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
	}
}
