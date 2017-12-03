#pragma strict
var middleSpine : Transform;
var master : Transform;
var aimPlus : float = 0;
var aimSidePlus : float = 0;
var mainCam : Transform;
var freeze : boolean = false;

function Start(){
	if(!master){
		master = transform.root;
	}
}

function LateUpdate (){
	if(!middleSpine || freeze || master.GetComponent(Status).freeze){
		return;
	}
	if(!mainCam){
		mainCam = Camera.main.transform;
	}
	
	middleSpine.localEulerAngles = new Vector3(middleSpine.localEulerAngles.x + aimSidePlus, middleSpine.localEulerAngles.y , -mainCam.localEulerAngles.x +aimPlus);
	//middleSpine.localEulerAngles = new Vector3(aimSidePlus, middleSpine.localEulerAngles.y , -mainCam.localEulerAngles.x +aimPlus);
}

@script AddComponentMenu ("Shooting RPG Kit/Rotate Spine")
