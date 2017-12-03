using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	public Transform target;
	public float targetHeight = 1.2f;
	public float targetSide = 0;
	public float distance = 4.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public float yMinLimit = -10;
	public float yMaxLimit = 70;
	private float x = 20.0f;
	private float y = 0.0f;
	public bool freeze = false;
	
	[HideInInspector]
		public float shakeValue = 0.0f;
	[HideInInspector]
		public bool  onShaking = false;
	private float shakingv = 0.0f;
	
	void Start (){
		if(!target){
			target = GameObject.FindWithTag ("Player").transform;
		}
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		Screen.lockCursor = true;
	}
	
	void LateUpdate (){
		if(!target)
			return;
		
		if (Time.timeScale == 0.0f || freeze){
			return;
		}
		
		x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
		y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		
		//Rotate Camera
		Quaternion rotation = Quaternion.Euler(y, x, 0);
		transform.rotation = rotation;
		
		//Rotate Target
		if(!freeze && target.GetComponent<Status>()){
			target.transform.rotation = Quaternion.Euler(0, x, 0);
		}
		
		//Camera Position
		//Vector3 neoTargetSide = transform.position - target.position;
		Vector3 position = target.position - (rotation * new Vector3(targetSide , 0 , 1) * distance + new Vector3(0,-targetHeight,0));
		transform.position = position;
		
		RaycastHit hit;
		Vector3 trueTargetPosition = target.position - new Vector3(targetSide,-targetHeight,0);
		
		if (Physics.Linecast (trueTargetPosition, transform.position, out hit)){
			if(hit.transform.tag == "Wall"){
				transform.position = hit.point + hit.normal*0.1f;   //put it at the position that it hit
			}
		}
		
		if(onShaking){
			shakeValue = Mathf.Lerp(shakeValue, shakingv, Time.deltaTime * 2);
			//shakeValue = Random.Range(-shakingv , shakingv)* 0.2f;
			//transform.position.y += shakeValue;
			transform.position = new Vector3(transform.position.x , transform.position.y + shakeValue , transform.position.z);
		}
	}
	
	static float ClampAngle(float angle , float min , float max){
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	
	public void Shake(float val , float dur){
		if(onShaking){
			return;
		}
		//Shaking(val , dur);
		StartCoroutine(Shaking(val , dur));
	}
	
	IEnumerator Shaking(float val , float dur){
		onShaking = true;
		shakingv = val;
		yield return new WaitForSeconds(dur);
		shakingv = 0;
		shakeValue = 0;
		onShaking = false;
	}
	
}