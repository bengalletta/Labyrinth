#pragma strict

enum AIState { Moving = 0, Pausing = 1 , Idle = 2 , MoveToPoint = 3}

private var mainModel : GameObject;
var useMecanim : boolean = false;
var animator : Animator; //For Mecanim
var followTarget : Transform;
var approachDistance  : float = 2.0f;
var detectRange : float = 15.0f;
//var detectRadius : float = 20.0f;
var lostSight : float = 100.0f;
var speed  : float = 4.0f;
var canHearGunSound : boolean = true;
var moveToPointDuration : float = 5.0; //Use when enemy hearing gun sound. It will done it's move when it cannot reach that point in time.
var movingAnimation : AnimationClip;
var idleAnimation : AnimationClip;
var attackAnimation : AnimationClip;
var hurtAnimation : AnimationClip;

@HideInInspector
var flinch : boolean = false;

var freeze : boolean = false;

var bulletPrefab : GameObject;
var attackPoint : Transform;
var attackEffect : GameObject;

var attackCast : float = 0.3;
var attackDelay : float = 0.5;

var followState : AIState = AIState.Idle;
private var distance : float = 0.0;
private var atk : int = 0;
private var matk : int = 0;
private var knock : Vector3 = Vector3.zero;
@HideInInspector
var cancelAttack : boolean = false;
private var attacking : boolean = false;
private var castSkill : boolean = false;
private var gos : GameObject[]; 

var attackVoice : AudioClip;
var hurtVoice : AudioClip;
var attackSoundRadius : float = 0; // Can attract the enemy to gun fire position.
private var moveToPoint : Vector3;
private var moveEnough : float = 0.0;
var usePathfinding : boolean = false; //Require Nav Mesh Agent

function Start () {
	gameObject.tag = "Enemy"; 
	if(!attackPoint){
		//attackPoint = this.transform;
		attackPoint = new GameObject().transform;
		attackPoint.position = transform.position;
		attackPoint.parent = this.transform;
	}
	mainModel = GetComponent(Status).mainModel;
	if(!mainModel){
		mainModel = this.gameObject;
	}
	GetComponent(Status).useMecanim = useMecanim;
	//Assign MainModel in Status Script
	GetComponent(Status).mainModel = mainModel;
		//Set ATK = Monster's Status
		atk = GetComponent(Status).atk;
		matk = GetComponent(Status).matk;
        
      	followState = AIState.Idle;
        
        if(!useMecanim){
      		//If using Legacy Animation
	      	mainModel.GetComponent.<Animation>().Play(idleAnimation.name);
	        GetComponent(Status).hurt = hurtAnimation;
        }else{
        	//If using Mecanim Animation
        	if(!animator){
				animator = mainModel.GetComponent(Animator);
			}
        }
        
		if(hurtVoice){
			GetComponent(Status).hurtVoice = hurtVoice;
		}
}

function GetDestination()
    {
        var destination : Vector3 = followTarget.position;
        destination.y = transform.position.y;
        return destination;
    }

function Update () {
	var stat : Status = GetComponent(Status);
	var controller : CharacterController = GetComponent(CharacterController);
	
	gos = GameObject.FindGameObjectsWithTag("Player"); 
    if (gos.Length > 0) {
			followTarget = FindClosest().transform;
	}
	if(useMecanim){
		animator.SetBool("hurt" , stat.flinch);
	}
	
	if (stat.flinch){
		cancelAttack = true;
		var knock = transform.TransformDirection(Vector3.back);
		controller.Move(knock * 5* Time.deltaTime);
		followState = AIState.Moving;
		return;
	}
	
	if(freeze || stat.freeze){
		return;
	}

	if(!followTarget){
		return;
	}
	if(usePathfinding){
		var agent : UnityEngine.AI.NavMeshAgent;
		agent = GetComponent(UnityEngine.AI.NavMeshAgent);
	}
	//-----------------------------------
	
		if (followState == AIState.Moving) {
			var lookTo : Vector3 = followTarget.position;
	     	lookTo.y = transform.position.y;
	  		transform.LookAt(lookTo);
				if(!useMecanim){
	            //If using Legacy Animation
	                mainModel.GetComponent.<Animation>().CrossFade(movingAnimation.name, 0.2f);
	            }else{
					animator.SetBool("run" , true);
				}
            if(!usePathfinding && (followTarget.position - transform.position).magnitude <= approachDistance || usePathfinding && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.1 && (followTarget.position - transform.position).magnitude <= approachDistance){
                followState = AIState.Pausing;
                if(!useMecanim){
                //If using Legacy Animation
                	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f); 
                }else{
					animator.SetBool("run" , false);
				}
                //----Attack----
                //Attack();
            }else if((followTarget.position - transform.position).magnitude >= lostSight){
            //Lost Sight
            	GetComponent(Status).health = GetComponent(Status).maxHealth;
            	if(!useMecanim){
                //If using Legacy Animation
                	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f); 
                }else{
					animator.SetBool("run" , false);
				}
                followState = AIState.Idle;
            }else{
            	if(usePathfinding){
     				PathFinding(followTarget.position);
     			}else{
     				var forward : Vector3 = transform.TransformDirection(Vector3.forward);
     				controller.Move(forward * speed * Time.deltaTime);
     			}
            }
        }else if (followState == AIState.Pausing){
        	Attack();
        		/*if(!useMecanim){
                //If using Legacy Animation
                	mainModel.animation.CrossFade(idleAnimation.name, 0.2f); 
                }else{
					animator.SetBool("run" , false);
				}*/
       			 var destinya : Vector3 = followTarget.position;
     			   destinya.y = transform.position.y;
  				   transform.LookAt(destinya);
  				   			   
            distance = (transform.position - GetDestination()).magnitude;
            if (distance > approachDistance) {
                followState = AIState.Moving;
            }
        }
        //----------------Idle Mode--------------
        else if(followState == AIState.Idle){
        	//mainModel.animation.CrossFade(idleAnimation.name, 0.2f);
  			var destinationheight : Vector3 = followTarget.position;
     			destinationheight.y = transform.position.y - destinationheight.y;
     		var getHealth : int = GetComponent(Status).maxHealth - GetComponent(Status).health;
     			
            distance = (transform.position - GetDestination()).magnitude;
            if (distance < detectRange && Mathf.Abs(destinationheight.y) <= 4 || getHealth > 0){
                followState = AIState.Moving;
            }
        }else if(followState == AIState.MoveToPoint){
        	//---------Move To Point---------------
        	//moveToPoint
        	
        	if(!useMecanim){
	        //If using Legacy Animation
	        	mainModel.GetComponent.<Animation>().CrossFade(movingAnimation.name, 0.2f);
	        }else{
				animator.SetBool("run" , true);
			}
			if(usePathfinding){
     			PathFinding(moveToPoint);
     		}else{
     			forward = transform.TransformDirection(Vector3.forward);
     			controller.Move(forward * speed * Time.deltaTime);
     		}
     		
        	destinya = moveToPoint;
     		destinya.y = transform.position.y;
  		    transform.LookAt(destinya);
  				   			   
            distance = (transform.position - GetDestination()).magnitude;
            var dist2 : float = (transform.position - destinya).magnitude;
            
            getHealth = GetComponent(Status).maxHealth - GetComponent(Status).health;
            if (distance < detectRange || getHealth > 0){
                followState = AIState.Moving;
            }else if(dist2 <= 1.5 || Time.time > moveEnough){
            	followState = AIState.Idle;
            	if(!useMecanim){
                //If using Legacy Animation
                	mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f); 
                }else{
					animator.SetBool("run" , false);
				}
            }
        }
//-----------------------------------
}

function SetDestination(des : Vector3){
	if(!canHearGunSound || followState == AIState.Moving || followState == AIState.Pausing){
		return;
	}
	moveToPoint = des;
	moveEnough = Time.time + moveToPointDuration;
	followState = AIState.MoveToPoint;
}

function Attack(){
	var hit : RaycastHit;
	if (Physics.Linecast (transform.position, followTarget.position , hit)) {
		if(hit.transform.tag == "Wall"){
			if(usePathfinding){
     			PathFinding(followTarget.position);
     		}
			followState = AIState.Pausing;
			return;
		}
	}
	cancelAttack = false;
	if(GetComponent(Status).flinch || GetComponent(Status).freeze || freeze || attacking){
		return;
	}
	//Set ATK = Monster's Status
	atk = GetComponent(Status).atk;
	matk = GetComponent(Status).matk;
		
		freeze = true;
		attacking = true;
		if(attackAnimation){
			if(!useMecanim){
	        	//If using Legacy Animation
	        	//mainModel.animation[attackAnimation.name].layer = 5;
				mainModel.GetComponent.<Animation>().PlayQueued(attackAnimation.name, QueueMode.PlayNow);
			}else{
				animator.Play(attackAnimation.name);
			}
		}
		yield WaitForSeconds(attackCast);
		if(GetComponent(Status).flinch){
			freeze = false;
			attacking = false;
			return;
		}
		attackPoint.transform.LookAt(followTarget);
		if(!cancelAttack){
			if(attackVoice){
				GetComponent.<AudioSource>().clip = attackVoice;
				GetComponent.<AudioSource>().Play();
			}
			if(attackSoundRadius > 0){
				GunSoundRadius(attackSoundRadius);
			}
			if(attackEffect){
				var eff : GameObject = Instantiate(attackEffect, attackPoint.transform.position , attackPoint.transform.rotation);
				eff.transform.parent = attackPoint.transform;
			}
			var bulletShootout : GameObject = Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation);
			bulletShootout.GetComponent(BulletStatus).Setting(atk , matk , "Enemy" , this.gameObject);
		}

		yield WaitForSeconds(attackDelay);
		freeze = false;
		attacking = false;
		//CheckDistance();
		followState = AIState.Moving;
	
}

function GunSoundRadius(radius : float){
	var hitColliders = Physics.OverlapSphere(transform.position, radius);
 		 
	for (var i = 0; i < hitColliders.Length; i++) {
		if(hitColliders[i].tag == "Enemy"){	  
	    	hitColliders[i].SendMessage("SetDestination" , transform.position);
	    }
	}
}

function CheckDistance(){
	if(!followTarget){
		if(!useMecanim){
	        //If using Legacy Animation
			mainModel.GetComponent.<Animation>().CrossFade(idleAnimation.name, 0.2f);  
		}else{
			animator.SetBool("run" , false);
		}
		followState = AIState.Idle;
		return;
	}
	var distancea : float = (followTarget.position - transform.position).magnitude;
	if (distancea <= approachDistance){
			var destinya : Vector3 = followTarget.position;
     		 destinya.y = transform.position.y;
  			 transform.LookAt(destinya);
              Attack();
     }else{
          followState = AIState.Moving;
          if(!useMecanim){
          //If using Legacy Animation
          	mainModel.GetComponent.<Animation>().CrossFade(movingAnimation.name, 0.2f);
          }else{
			animator.SetBool("run" , true);
		  }
       }
}


function FindClosest() : GameObject { 
    // Find Closest Player   
   // var gos : GameObject[]; 
    gos = GameObject.FindGameObjectsWithTag("Player");
    gos += GameObject.FindGameObjectsWithTag("Ally"); 
    if(!gos){
    	return;
    }
    var closest : GameObject; 
    
    var distance : float = Mathf.Infinity; 
    var position : Vector3 = transform.position; 

    for (var go : GameObject in gos) { 
       var diff : Vector3 = (go.transform.position - position); 
       var curDistance : float = diff.sqrMagnitude; 
       if (curDistance < distance) { 
       //------------
         closest = go; 
         distance = curDistance; 
       } 
    } 
  //  var target = closest;
    return closest; 
}


function UseSkill(skill : Transform , castTime : float , delay : float , anim : String , dist : float){
	cancelAttack = false;
	if(flinch || !followTarget || (followTarget.position - transform.position).magnitude >= dist || GetComponent(Status).silence || GetComponent(Status).freeze  || castSkill){
		return;
	}
	
		freeze = true;
		castSkill = true;
		if(!useMecanim){
	        //If using Legacy Animation
	        mainModel.GetComponent.<Animation>()[anim].layer = 10;
			mainModel.GetComponent.<Animation>().Play(anim);
		}else{
			animator.Play(anim);
		}
		yield WaitForSeconds(castTime);
		if(flinch){
			freeze = false;
			castSkill = false;
			return;
		}
		attackPoint.transform.LookAt(followTarget);
		if(!cancelAttack){
			var bulletShootout : Transform = Instantiate(skill, attackPoint.transform.position , attackPoint.transform.rotation);
			bulletShootout.GetComponent(BulletStatus).Setting(atk , matk , "Enemy" , this.gameObject);
		}

		yield WaitForSeconds(delay);
		freeze = false;
		castSkill = false;
		if(!useMecanim){
	        //If using Legacy Animation
			mainModel.GetComponent.<Animation>().CrossFade(movingAnimation.name, 0.2f);
		}else{
			animator.SetBool("run" , true);
		}
	
}

function SetLevel(lv : int){
	var stat : Status = GetComponent(Status);
	stat.level = lv;
	
	var autoLv : AutoCalculateStatus = GetComponent(AutoCalculateStatus);
	if(autoLv){
		autoLv.CalculateStatLv();
	}
	detectRange = 500;
}

function PathFinding(target : Vector3) {
	//Require Nav Mesh Agent.
	var agent : UnityEngine.AI.NavMeshAgent;
	agent = GetComponent(UnityEngine.AI.NavMeshAgent);
	agent.SetDestination (target);
}

@script RequireComponent (Status)
@script RequireComponent (CharacterMotor)

@script AddComponentMenu ("Shooting RPG Kit/Create Enemy")
