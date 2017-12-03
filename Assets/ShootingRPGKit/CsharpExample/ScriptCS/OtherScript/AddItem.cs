using UnityEngine;
using System.Collections;

public class AddItem : MonoBehaviour {
	public int itemID = 0;
	public int itemQuantity = 1;
	private Transform master;
	
	public enum ItType {
		Usable = 0,
		Equipment = 1,
	}
	
	public ItType itemType = ItType.Usable; 
	
	public float duration = 30.0f;
	
	void  Start (){
		master = transform.root;
		if(duration > 0){
			Destroy (master.gameObject, duration);
		}
	}
	
	void OnTriggerEnter(Collider other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			AddItemToPlayer(other.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			AddItemToPlayer(other.gameObject);
		}
	}
	
	void AddItemToPlayer(GameObject other){
		bool full = false;
		if(itemType == ItType.Usable){
			full = other.GetComponent<Inventory>().AddItem(itemID , itemQuantity);
		}else{
			full = other.GetComponent<Inventory>().AddEquipment(itemID , itemQuantity);
		}
		
		if(!full){
			master = transform.root;
			Destroy(master.gameObject);
		}
	}
	
	
	
}