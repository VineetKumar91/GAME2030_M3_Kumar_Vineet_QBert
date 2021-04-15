using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // returns reference to self by instantiating it, does not require an instance to get called
    // essentially a manager
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null)
                _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }
}
