#pragma strict
var enemy : Transform;
//var point = 10;
var pointName : String = "SpawnPoint";
var delay : float = 3.0;
var randomPoint : float = 10.0;

function Start () {
	var spawnpoints : GameObject[] = GameObject.FindGameObjectsWithTag (pointName);
	if(spawnpoints.Length > 0){
			var spawnpoint : Transform = spawnpoints[Random.Range(0, spawnpoints.length)].transform;
		
			yield WaitForSeconds (delay);
			var ranPos : Vector3 = spawnpoint.position; //Slightly Random x z position from respawn point.
			ranPos.x += Random.Range(0.0,randomPoint);
			ranPos.z += Random.Range(0.0,randomPoint);
			var m : Transform = Instantiate(enemy, ranPos , spawnpoint.rotation);
			m.name = enemy.name;
			Destroy (gameObject, 1);
	}else{
			Destroy (gameObject, delay+1);
	}
	
}