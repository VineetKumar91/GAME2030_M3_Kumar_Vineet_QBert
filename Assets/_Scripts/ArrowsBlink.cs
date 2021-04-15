using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowsBlink : MonoBehaviour
{
    // Note: Not Using this script since deactivating object also deactivates script, Trying to find alternatives
    // Note: Tried Render, but gamemgr preferable
    public float InsideArrowDelay = 1.0f;
    public float OutsideArrowDelay = 0.5f;

    public bool bKeepOnBlinking = true;

    private GameObject[] insideArrows = new GameObject[2];
    private GameObject[] outsideArrows = new GameObject[2];

    void Start()
    {
        insideArrows[0] = GameObject.Find("InsideArrow_Left");
        insideArrows[1] = GameObject.Find("InsideArrow_Right");
        outsideArrows[0] = GameObject.Find("OutsideArrow_Left");
        outsideArrows[1] = GameObject.Find("OutsideArrow_Right");

       /* for (int i = 0; i < 2; i++)
        {
            StartCoroutine(BlinkArrow(insideArrows[i], InsideArrowDelay,1));
            StartCoroutine(BlinkArrow(outsideArrows[i], OutsideArrowDelay,2));
        }*/
    }

    /*public IEnumerator BlinkArrow(GameObject gameObject, float seconds, int type)
    {
        // Three animations for the arrows.
        // Outside Arrows Visible
        // Outside - Inside Arrows visible
        // None of Arrows Visible
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        while(bKeepOnBlinking)
        {
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(seconds);
        }
    }*/
}
