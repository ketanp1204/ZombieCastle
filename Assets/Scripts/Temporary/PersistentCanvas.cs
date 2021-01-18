using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas instance;

    void Awake()
    {
        Debug.Log(gameObject.name + instance);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("destroying " + gameObject.name + " canvas");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
