#pragma strict
var target1 : Transform;
var target2 : Transform;
private var target : Transform;
var speed : float = 4.0f;
var lookAtTarget : boolean = false;
var stayDuration : float = 2.5;
private var wait : float = 0;
private var moving : boolean = true;
	
function Start () {
	//Release Target Object from Parent
	target1.transform.parent = null;
	target2.transform.parent = null;
	//Set Target
	target = target1;
}

function FixedUpdate(){
	if(moving){
		var step : float = speed * Time.deltaTime;
	    transform.position = Vector3.MoveTowards(transform.position, target.position, step);
		if(transform.position == target.transform.position){
			//Set New Target
			if(target == target1){
				target = target2;
			}else{
				target = target1;	
			}
			if(lookAtTarget){
				transform.LookAt(target);
			}
			moving = false;
		}
	}else{
		if(wait >= stayDuration){
			moving = true;
			wait = 0;
		}else{
			wait += Time.deltaTime;
		}
	}
}

