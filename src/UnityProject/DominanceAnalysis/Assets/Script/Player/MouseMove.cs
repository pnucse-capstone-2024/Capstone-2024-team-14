using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    public float sensitivity = 100.0f;
    public float rotationX;
    public float rotationY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseMoveX = Input.GetAxis("Mouse X");
        float mouseMoveY = Input.GetAxis("Mouse Y");

        rotationY += mouseMoveX * sensitivity * Time.deltaTime;
        rotationX += mouseMoveY * sensitivity * Time.deltaTime;

        if (rotationX > 35.0f)
        {
            rotationX = 35.0f;
        }

        if (rotationX < -30.0f)
        {
            rotationX = -30.0f;
        }

        transform.eulerAngles = new Vector3(-rotationX, rotationY, 0.0f);
    }
}
