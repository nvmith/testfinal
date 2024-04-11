using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Hand : MonoBehaviour
{
	float angle;
	Vector2 target, mouse;

	void Start()
    {
		target = transform.position;
	}

    void Update()
    {
		LookMouse();
	}

	void LookMouse()
	{
		mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
