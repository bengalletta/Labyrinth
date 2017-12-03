#pragma strict
var tip : Texture2D;
private var show : boolean = false;

function Update () {
		if(Input.GetKeyDown("h")){
			if(show){
				show = false;
			}else{
				show = true;
			}
		}

}

function OnGUI () {
	if(show){
		GUI.DrawTexture(Rect(3,205,210,252), tip);
	}
}