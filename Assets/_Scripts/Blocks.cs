using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite Block;
    public Sprite BlockChanged;
    private bool isChanged = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Activate()
    {
        if (!isChanged)
        {
            spriteRenderer.sprite = BlockChanged;
            isChanged = true;
            // Score for Cube
            GameManager.CurrentScore += 25;
            GameManager.BlocksRemainingCount -= 1;
            //Debug.Log("Number of Blocks to go: " + GameManager.BlocksRemainingCount);
        }

    }
}
