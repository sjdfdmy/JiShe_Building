using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickManager : MonoBehaviour
{
    private static PickManager instance;
    public static PickManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<PickManager>();
                if (instance == null)
                {
                    Debug.Log("No PickSystem");
                }
            }
            return instance;
        }
    }
    public GameObject picks;
    public Transform GetObjs;

    void Start()
    {
        picks.SetActive(false);
    }

    void Update()
    {
        
    }
}
