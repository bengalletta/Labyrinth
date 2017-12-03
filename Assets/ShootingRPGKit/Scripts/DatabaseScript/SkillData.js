#pragma strict

class Skil {
	var skillName : String = "";
	var icon : Texture2D;
	var description : String = "";
	var skillPrefab : Transform;
	var skillAnimation : AnimationClip;
	var manaCost : int = 10;
	var castTime : float = 0.3;
	var skillDelay : float = 0.3;
}

var skill : Skil[] = new Skil[3];