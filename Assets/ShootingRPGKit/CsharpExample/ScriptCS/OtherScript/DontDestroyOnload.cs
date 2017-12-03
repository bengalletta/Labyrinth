using UnityEngine;
using System.Collections;

public class DontDestroyOnload : MonoBehaviour {
	
	void  Awake (){
		this.transform.parent = null;
		DontDestroyOnLoad (transform.gameObject);
	}
}