using UnityEngine;
using System.Collections;

[AddComponentMenu("Shooting-RPG Kit(CS)/Rotate Spine")]
public class RotateSpine : MonoBehaviour {
	public Transform middleSpine;
	public Transform master;
	public float aimPlus = 0;
	public float aimSidePlus = 0;
	public Transform mainCam;
	public bool freeze = false;
	
	void Start (){
		if(!master){
			master = transform.root;
		}
	}
	
	void LateUpdate (){
		if(!middleSpine || freeze || master.GetComponent<Status>().freeze){
			return;
		}
		if(!mainCam){
			mainCam = Camera.main.transform;
		}
		
		middleSpine.localEulerAngles = new Vector3(middleSpine.localEulerAngles.x + aimSidePlus, middleSpine.localEulerAngles.y , -mainCam.localEulerAngles.x +aimPlus);
		//middleSpine.localEulerAngles = new Vector3(aimSidePlus, middleSpine.localEulerAngles.y , -mainCam.localEulerAngles.x +aimPlus);
	}
		
}