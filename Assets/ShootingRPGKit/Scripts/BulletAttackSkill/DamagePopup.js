#pragma strict
var targetScreenPosition : Vector3;
var damage : String = "";
var fontStyle : GUIStyle;
var criticalFontStyle : GUIStyle;

var duration : float = 0.5;

private var glide : int = 50;

@HideInInspector
var critical : boolean = false;

function Start () {
	Destroy (gameObject, duration);
	
	var a : int = 0;
	while(a < 100){
		glide += 2;
		yield WaitForSeconds(0.03); 
	}
}

function OnGUI(){
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