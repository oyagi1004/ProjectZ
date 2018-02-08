#pragma strict

 var speed = 5;

function Update () {
transform.localEulerAngles.y += speed * Time.deltaTime ;
}