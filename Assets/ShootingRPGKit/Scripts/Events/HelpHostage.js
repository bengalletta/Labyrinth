#pragma strict
var hostage : Transform;
@HideInInspector
var player : GameObject;

var operatorObj : GameObject;
var operatorMessage : OperatorSet[] = new OperatorSet[1];

var activateObj : GameObject[];

function Start () {
	if(!hostage){
		hostage = transform.root;
	}
	hostage.GetComponent(AIfriend).enabled = false;
}

function Rescue(){
	player = GetComponent(Dialogue).player;
	if(!player){
		player = GameObject.FindWithTag("Player");
	}
	hostage.GetComponent(AIfriend).master = player.transform;
	hostage.GetComponent(AIfriend).enabled = true;
	
	var hb : AllyHealthBar = hostage.GetComponent(AllyHealthBar);
	if(hb){
		hb.enabled = true;
	}
	
	if(!operatorObj){
		operatorObj = GameObject.Find("Mission");
	}
	operatorObj.GetComponent(Operator).otherMessage = operatorMessage;
	operatorObj.GetComponent(Operator).ShowOtherMessage();
	if(activateObj.Length > 0){
		ActivateObject();
	}
	Destroy(gameObject);
}

function ActivateObject(){
	for (var ob : GameObject in activateObj) {
    	ob.SetActive(true);
    }
}

@script RequireComponent(Dialogue)
