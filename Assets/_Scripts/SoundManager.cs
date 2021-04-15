using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource ballJump_Src;
    public AudioSource coilyFall_Src;
    public AudioSource coilyJump_Src;
    public AudioSource elevator_Src;
    public AudioSource qbertFall_Src;
    public AudioSource qbertJump_Src;
    public AudioSource qbertSwear_Src;
    public AudioSource greenBallActivated_Src;

    public AudioSource victory_Src;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play_BallJump_Src()
    {
        ballJump_Src.Play();
    }

    public void Play_CoilyFall_Src()
    {
        coilyFall_Src.Play();
    }

    public void Play_CoilyJump_Src()
    {
        coilyJump_Src.Play();
    }

    public void Play_Elevator_Src()
    {
        elevator_Src.Play();
    }

    public void Play_QbertFall_Src()
    {
        qbertFall_Src.Play();
    }

    public void Play_QbertJump_Src()
    {
        qbertJump_Src.Play();
    }

    public void Play_QbertSwear_Src()
    {
        qbertSwear_Src.Play();
    }

    public void Play_Victory_Src()
    {
        victory_Src.Play();
    }

    public void Play_GreenBallActivated_Src()
    {
        greenBallActivated_Src.Play();
    }
}
