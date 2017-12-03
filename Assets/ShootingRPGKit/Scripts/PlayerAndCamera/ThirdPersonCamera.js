#pragma strict
var target : Transform;
var targetHeight : float = 1.2;
var targetSide : float = 0;
var distance : float = 4.0;
var xSpeed : float = 250.0;
var ySpeed : float = 120.0;
var yMinLimit : float = -10;
var yMaxLimit : float = 70;
private var x : float = 20.0;
private var y : float = 0.0;
var freeze : boolean = false;

@HideInInspector
var shakeValue : float = 0.0;
@HideInInspector
var onShaking : boolean = false;
private var shakingv : float = 0.0;
         
function Start () {
     if(!target){
    	target = GameObject.FindWithTag ("Player").transform;
     }
        var angles : Vector3 = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
     
          if (GetComponent.<Rigidbody>())
          GetComponent.<Rigidbody>().freezeRotation = true;
          Screen.lockCursor = true;
}
     
function LateUpdate () {
       if(!target)
          return;
          
    if (Time.timeScale == 0.0 || freeze){
		return;
	}
       
         x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
      	 y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
       
       y = ClampAngle(y, yMinLimit, yMaxLimit);
       
       //Rotate Camera
       var rotation:Quaternion = Quaternion.Euler(y, x, 0);
       transform.rotation = rotation;
       
        //Rotate Target
        if(!freeze && target.GetComponent(Status)){
        	target.transform.rotation = Quaternion.Euler(0, x, 0);
        }
       
       //Camera Position
       var neoTargetSide : Vector3 = transform.position - target.position;
       var position : Vector3 = target.position - (rotation * Vector3(targetSide , 0 , 1) * distance + Vector3(0,-targetHeight,0));
       transform.position = position;
       
        var hit : RaycastHit;
        var trueTargetPosition : Vector3 = target.position - Vector3(targetSide,-targetHeight,0);

		if (Physics.Linecast (trueTargetPosition, transform.position, hit)){
            if(hit.transform.tag == "Wall"){
    			transform.position = hit.point + hit.normal*0.1f;   //put it at the position that it hit
    		}
		}
        
        if(onShaking){
        	shakeValue = Mathf.Lerp(shakeValue, shakingv, Time.deltaTime * 2);
        	//shakeValue = Random.Range(-shakingv , shakingv)* 0.2;
        	transform.position.y += shakeValue;
        }
}
     
static function ClampAngle (angle : float, min : float, max : float) {
       if (angle < -360)
          angle += 360;
       if (angle > 360)
          angle -= 360;
       return Mathf.Clamp (angle, min, max);
       
}


function Shake(val : float , dur : float){
	if(onShaking){
		return;
	}
	Shaking(val , dur);
}

function Shaking(val : float , dur : float){
	onShaking = true;
	shakingv = val;
	yield WaitForSeconds(dur);
	shakingv = 0;
	shakeValue = 0;
	onShaking = false;
}
