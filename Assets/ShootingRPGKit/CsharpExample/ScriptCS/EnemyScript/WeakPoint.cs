using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour {
	public Transform master;
	public int damagePercent = 200;
	public bool isCritical = true;
	public bool ignoreGuard = true;
	// Use this for initialization
	void Start () {
		if(!master){
			master = transform.root;
		}
		this.tag = "WeakPoint";
	}

}
