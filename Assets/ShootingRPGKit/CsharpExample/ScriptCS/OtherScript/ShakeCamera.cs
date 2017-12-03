using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour{
	public float shakeValue = 0.3f;
	public float shakeDuration = 0.3f;
	
	void  Start (){
		Camera.main.GetComponent<ThirdPersonCamera>().Shake(shakeValue , shakeDuration);
	}
}