using UnityEngine;
using System.Collections;

public class DamagePopup : MonoBehaviour {
	public Vector3 targetScreenPosition;
	public string damage = "";
	public GUIStyle fontStyle;
	public GUIStyle criticalFontStyle;

	public float duration = 0.5f;
	
	private int glide = 50;

	[HideInInspector]
		public bool critical = false;
	
	void Start (){
		Destroy (gameObject, duration);
		StartCoroutine(CC());

	}
	IEnumerator CC(){
		int a = 0;
		while(a < 100){
			glide += 2;
			yield return new WaitForSeconds(0.03f); 
		}
	}
	
	void  OnGUI (){
		targetScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
		targetScreenPosition.y = Screen.height - targetScreenPosition.y - glide;
		targetScreenPosition.x = targetScreenPosition.x - 6;
		if(targetScreenPosition.z > 0){
			if(critical){
				GUI.Label (new Rect (targetScreenPosition.x,targetScreenPosition.y,100,50), damage,criticalFontStyle);
			}else{
				GUI.Label (new Rect (targetScreenPosition.x,targetScreenPosition.y,100,50), damage,fontStyle);
			}
		}
	}
}