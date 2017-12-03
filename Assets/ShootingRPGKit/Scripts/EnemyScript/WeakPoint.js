#pragma strict
var master : Transform;
var damageMultiply : float = 2;
var isCritical : boolean = true;
var ignoreGuard : boolean = true;

function Start (){
	if(!master){
		master = transform.root;
	}
	this.tag = "WeakPoint";
}
