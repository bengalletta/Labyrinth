using UnityEngine;
using System.Collections;

[AddComponentMenu("Shooting-RPG Kit(CS)/Upper Body Animation Mix(Legacy)")]
public class UpperBodyAnimationMix : MonoBehaviour {
	
	private GameObject mainModel;
	public Transform upperBody;
	public AnimationClip[] animationFile = new AnimationClip[1];
	
	void Start(){
		//For Legacy Animation.
		if(!mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}
		int c = 0;
		if(animationFile.Length > 0){
			while(c < animationFile.Length && animationFile[c]){
				mainModel.GetComponent<Animation>()[animationFile[c].name].AddMixingTransform(upperBody);
				c++;
			}
		}
	}
			
}
