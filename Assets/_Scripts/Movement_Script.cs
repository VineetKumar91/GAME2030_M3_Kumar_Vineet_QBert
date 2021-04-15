using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Script : MonoBehaviour
{

    private GameObject sprite_instance = null;
    public Vector3 position0 = Vector3.zero;
    public Vector3 position1 = Vector3.zero;
    public Vector3 position2 = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveAlongBezierCurve(GameObject spriteInstance, Vector3 startPosition, Vector3 endPosition)
    {
        //Debug.Log("In Move Bezier Curve Function!");
        position0 = startPosition;
        position2 = endPosition;
        position1 = position0 + (position2 - position0) / 2 + Vector3.up * 1 ;
        sprite_instance = spriteInstance;

        Vector3 m1 = Vector3.Lerp(position0, position1, 1 * Time.deltaTime);
        Vector3 m2 = Vector3.Lerp(position1, position2, 1 * Time.deltaTime);
        spriteInstance.GetComponent<Transform>().localPosition = Vector3.Lerp(m1, m2, 1 * Time.deltaTime);
    }
}
