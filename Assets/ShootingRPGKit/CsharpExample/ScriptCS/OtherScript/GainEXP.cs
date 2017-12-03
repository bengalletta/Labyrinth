using UnityEngine;
using System.Collections;

public class GainEXP : MonoBehaviour {
	
	public int expGain = 20;
	GameObject player;
	void  Start (){
		if(!player){
			player = GameObject.FindWithTag ("Player");
		}
		player.GetComponent<Status>().gainEXP(expGain);
	}
}