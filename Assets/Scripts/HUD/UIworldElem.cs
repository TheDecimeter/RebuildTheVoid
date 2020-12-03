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
        modelInstance.transform.position = pos();// cam.ScreenToWorldPoint(uiSpace.transform.position);
        modelInstance.transform.forward = (cam.transform.forward*3+cam.transform.up).normalized;
    }

    Vector3 pos()
    {
        Vector3 r=cam.transform.position-cam.ScreenToWorldPoint(uiSpace.transform.position);
        return cam.transform.position-r.normalized * 7;
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
