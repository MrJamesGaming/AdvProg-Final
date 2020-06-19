using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour{

    public Vector3 targetPos;

    public float speed = 0.1f;

    public float arcHeight = 1;

    

    Vector3 startPos;

    //called at the start
    void Start(){
        //start position
        startPos = transform.position;
        targetPos = new Vector3(2,0,-6);
        Debug.Log("Start Z: " + transform.rotation.z * Mathf.Rad2Deg);
        Debug.Log("Start W: " + transform.rotation.w * Mathf.Rad2Deg);
    }

    //called each frame
    void Update(){
        // Compute the next position, with arc added in
        float x0 = startPos.x;
        float x1 = targetPos.x;
        float dist = x1 - x0;
        float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
        float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
        float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
        Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
        
        // Rotate to face the next position, and then move there
        transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;

        // Do something when we reach the target
        if (nextPos == targetPos){
            Arrived();
        }
    }

    void Arrived(){
        Destroy(gameObject);
    }

    /// 0,1,-10 
    /// This is a 2D version of Quaternion.LookAt; it returns a quaternion
    /// that makes the local +X axis point in the given forward direction.
    /// 
    /// forward direction
    /// Quaternion that rotates +X to align with forward
    static Quaternion LookAt2D(Vector2 forward){
        return Quaternion.Euler(0 , 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}
