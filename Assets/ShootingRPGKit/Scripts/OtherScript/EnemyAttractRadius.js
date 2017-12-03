#pragma strict
var radius : float = 30.0;

function Start () {
	var hitColliders = Physics.OverlapSphere(transform.position, radius);
 		 
	for (var i = 0; i < hitColliders.Length; i++) {
		if(hitColliders[i].tag == "Enemy"){	  
	    	hitColliders[i].SendMessage("SetDestination" , transform.position);
	    }
	}
}

