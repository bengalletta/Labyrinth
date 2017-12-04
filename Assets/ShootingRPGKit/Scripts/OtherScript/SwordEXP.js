#pragma strict


var addAmmo : AllAmmo;
var expGain = 250;
var duration : float = 30.0;
private var master : Transform;

function Start () {

	
	master = transform.root;
	if(duration > 0){
		Destroy (master.gameObject, duration);
	}
}

function OnTriggerEnter (other : Collider) {
		//Pick up Item
	if (other.gameObject.tag == "Player") {
		AddAmmoToPlayer(other.gameObject);

		var player : GameObject[];
   	 	player = GameObject.FindGameObjectsWithTag("Player");
    	player += GameObject.FindGameObjectsWithTag("Ally");
    	for (var pl : GameObject in player) { 
  			 	if(pl){
  			 		pl.GetComponent(Status).gainEXP(expGain);
  			 	}
  			 }


     }
}
 
function OnCollisionEnter(other : Collision) {
		//Pick up Item
	if (other.gameObject.tag == "Player") {
		AddAmmoToPlayer(other.gameObject);
     }
}
 
function AddAmmoToPlayer(other : GameObject){
	other.GetComponent(GunTrigger).allAmmo.handgunAmmo += addAmmo.handgunAmmo;
	other.GetComponent(GunTrigger).allAmmo.machinegunAmmo += addAmmo.machinegunAmmo;
	other.GetComponent(GunTrigger).allAmmo.shotgunAmmo += addAmmo.shotgunAmmo;
	other.GetComponent(GunTrigger).allAmmo.magnumAmmo += addAmmo.magnumAmmo;
	other.GetComponent(GunTrigger).allAmmo.smgAmmo += addAmmo.smgAmmo;
	other.GetComponent(GunTrigger).allAmmo.sniperRifleAmmo += addAmmo.sniperRifleAmmo;
	other.GetComponent(GunTrigger).allAmmo.grenadeRounds += addAmmo.grenadeRounds;
 	
 	master = transform.root;
    Destroy(master.gameObject);
}