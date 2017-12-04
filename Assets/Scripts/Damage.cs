using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Damage : MonoBehaviour {
	public LayerMask mask;
	public float maxDistance = 2.2f;
	public string methodName = "Damage";
	public float damageAmount = 1.0f;
	public Transform spawnPoint = null;

	private void SendDamage(float damage){
		RaycastHit hit;
		damage = damageAmount;

		if (Physics.Raycast (spawnPoint.position, spawnPoint.forward, out hit, maxDistance, mask)) {
			hit.collider.SendMessageUpwards (methodName, damage, SendMessageOptions.DontRequireReceiver);
		}

	}
}