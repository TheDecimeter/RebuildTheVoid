using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfHTML : MonoBehaviour
{
#if UNITY_WEBGL
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

#endif
}
