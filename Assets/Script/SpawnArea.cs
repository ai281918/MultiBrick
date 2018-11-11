using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnArea : MonoBehaviour {
	bool pressed = false;
	public UnityEvent pressEvent = new UnityEvent();

	// Update is called once per frame
	void Update () {
		if(pressed){
			pressEvent.Invoke();
		}
	}

	public void PointerDown()
	{
		pressed = true;
		pressEvent.Invoke();
	}

	public void PointerUp()
	{
		pressed = false;
	}
}
