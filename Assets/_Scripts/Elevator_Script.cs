using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator_Script : MonoBehaviour
{
    [SerializeField] 
    private Vector3 finalPosition;

    private Vector3 startingPosition;

    private Vector3 NormalizedDirection;

    [SerializeField] private GameObject Qbert; 
    private Qbert_Script QbertScript;

    [SerializeField] public bool ActivateElevator = false;
    [SerializeField] private bool ElevatorReachedTimeToDestroy = false;

    // Sounds
    private SoundManager SoundManagerReference;
    private bool playSoundOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Elevator Script Activated :) !!!");
        startingPosition = this.transform.position;
        NormalizedDirection = finalPosition - startingPosition;
        NormalizedDirection = Vector3.Normalize(NormalizedDirection);
        QbertScript = GameObject.FindGameObjectWithTag("Qbert").GetComponent<Qbert_Script>();

        // Sounds
        SoundManagerReference = GameObject.FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ActivateElevator)
        {
            MoveElevator();
        }

        if (ElevatorReachedTimeToDestroy)
        {
            StartCoroutine(DestroyElevator());
            this.gameObject.GetComponent<Animator>().enabled = false;
            ElevatorReachedTimeToDestroy = false;
        }
    }

    IEnumerator DestroyElevator()
    {
        GameManager.ElevatorCount -= 1;
        //Debug.Log("Elevators remaining = " + GameManager.ElevatorCount);
        yield return new WaitForSeconds(1.1f);
        QbertScript.PostElevatorMovement();
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    }

    public void MoveElevator()
    {
        //Debug.Log("In IEnumerator");
        //this.transform.position = Vector3.MoveTowards(this.transform.position, finalPosition, 1 * Time.deltaTime);
        
        this.transform.Translate(NormalizedDirection * Time.deltaTime);
        QbertScript.isOnElevator = true;
        Qbert.transform.Translate(NormalizedDirection * Time.deltaTime);
        if (Vector3.Distance(this.transform.position, finalPosition) <= 0.1)
        {
            //Debug.Log("Deactivating Moving");
            ActivateElevator = false;
            ElevatorReachedTimeToDestroy = true;
            //Debug.Log("While Loop ");
        }

        if (!playSoundOnce)
        {
            playSoundOnce = true;
            SoundManagerReference.Play_Elevator_Src();
        }

        //this.transform.Translate();
    }

}
