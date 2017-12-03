using UnityEngine;
using System.Collections;

public class WIn : MonoBehaviour {

	private bool showName = false;

	void OnTriggerEnter2D()
	{
		showName = true;
	}
	void OnTriggerExit2D()
	{
		showName =false;
	}

	void OnGUI()
	{
		if(showName)
			GUI.Label(new Rect(10,10,100,100), transform.name);
	}

}