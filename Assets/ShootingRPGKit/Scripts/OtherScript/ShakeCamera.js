#pragma strict
var shakeValue : float = 0.3;
var shakeDuration : float = 0.3;

function Start () {
	GetComponent.<Camera>().main.GetComponent(ThirdPersonCamera).Shake(shakeValue , shakeDuration);
}
