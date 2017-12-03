#pragma strict
private var mainModel : GameObject;
var upperBody : Transform;
var animationFile : AnimationClip[] = new AnimationClip[1];

function Start () {
		//For Legacy Animation.
		if(!mainModel){
			mainModel = GetComponent(Status).mainModel;
		}
		var c : int = 0;
		if(animationFile.Length > 0){
			while(c < animationFile.Length && animationFile[c]){
				mainModel.GetComponent.<Animation>()[animationFile[c].name].AddMixingTransform(upperBody);
				c++;
			}
		}
}

@script AddComponentMenu ("Shooting RPG Kit/Upper Body Animation Mix(Legacy)")
