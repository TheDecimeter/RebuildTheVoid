using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIworldElem : MonoBehaviour
{
    public GameObject Model;
    private GameObject modelInstance;
    public RectTransform uiSpace;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        modelInstance.transform.position=cam.ScreenToWorldPoint(uiSpace.transform.position);
        modelInstance.transform.forward = cam.transform.forward;
    }

    private void OnEnable()
    {
        modelInstance = Instantiate(Model);
        modelInstance.name = "uiDisplay";
    }
    private void OnDisable()
    {
        Destroy(modelInstance);
    }
}
