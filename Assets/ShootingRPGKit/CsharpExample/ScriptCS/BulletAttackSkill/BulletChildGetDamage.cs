using UnityEngine;
using System.Collections;

public class BulletChildGetDamage : MonoBehaviour {
	public GameObject master;
	
	void  Start (){
		GetComponent<BulletStatus>().totalDamage = master.GetComponent<BulletStatus>().totalDamage;
		GetComponent<BulletStatus>().shooterTag = master.GetComponent<BulletStatus>().shooterTag;
		
	}
	
}