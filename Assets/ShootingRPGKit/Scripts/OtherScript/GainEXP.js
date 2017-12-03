#pragma strict
var expGain = 20;
function Start () {
    var player : GameObject[];
    player = GameObject.FindGameObjectsWithTag("Player");
    player += GameObject.FindGameObjectsWithTag("Ally");
    for (var pl : GameObject in player) { 
  			 	if(pl){
  			 		pl.GetComponent(Status).gainEXP(expGain);
  			 	}
  			 }
}
