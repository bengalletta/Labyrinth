#pragma strict
var itemID : int = 0;
var itemQuantity : int = 1;
private var master : Transform;

enum ItType {
	Usable = 0,
	Equipment = 1,
}

var itemType : ItType = ItType.Usable; 

var duration : float = 30.0;

function Start () {
	master = transform.root;
	if(duration > 0){
		Destroy (master.gameObject, duration);
	}
}

function OnTriggerEnter (other : Collider) {
		//Pick up Item
	if (other.gameObject.tag == "Player") {
		AddItemToPlayer(other.gameObject);
     }
 }
 
function OnCollisionEnter(other : Collision) {
		//Pick up Item
	if (other.gameObject.tag == "Player") {
		AddItemToPlayer(other.gameObject);
     }
}
 
function AddItemToPlayer(other : GameObject){
 	if(itemType == ItType.Usable){
			var full : boolean = other.GetComponent(Inventory).AddItem(itemID , itemQuantity);
		}else{
			full = other.GetComponent(Inventory).AddEquipment(itemID , itemQuantity);
		}
 		
		if(!full){
			master = transform.root;
    		Destroy(master.gameObject);
    	}
}

 
