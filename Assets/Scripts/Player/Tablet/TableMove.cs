using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableMove : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset;
    public Vector3 orientation;
    public Vector3 center;
    public float moveSpeed;
    // private GameObject Buttom0;
    // private GameObject Buttom1;
    // private GameObject Buttom2;
    // private GameObject Buttom3;

    // Start is called before the first frame update
    void Start()
    {
        orientation = new Vector3(0.0f, 0.0f, 90.0f);
//        Buttom0 = transform.Find("Buttom0").gameObject;
//        Buttom1 = transform.Find("Buttom1").gameObject;
//        Buttom2 = transform.Find("Buttom2").gameObject;
//        Buttom3 = transform.Find("Buttom3").gameObject; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            offset = Quaternion.AngleAxis(-1.0f*moveSpeed, Vector3.up) * offset;
        }
        if (Input.GetKey(KeyCode.J))
        {
            offset = Quaternion.AngleAxis(-1.0f*moveSpeed, Vector3.right) * offset;
        }
        if (Input.GetKey(KeyCode.K))
        {
            offset = Quaternion.AngleAxis(moveSpeed, Vector3.right) * offset;
        }
        if (Input.GetKey(KeyCode.L))
        {
            offset = Quaternion.AngleAxis(moveSpeed, Vector3.up) * offset;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            if (orientation.z == 0.0f)
            {
                orientation.z = 90.0f;
            }
            else
            {
                orientation.z = 0.0f;
            }
        }

        // if (Input.GetKeyDown(KeyCode.Alpha0))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom0.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.red);
        // }
        // if (Input.GetKeyUp(KeyCode.Alpha0))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom0.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.white);
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom1.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.red);
        // }
        // if (Input.GetKeyUp(KeyCode.Alpha1))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom1.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.white);
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom2.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.red);
        // }
        // if (Input.GetKeyUp(KeyCode.Alpha2))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom2.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.white);
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom3.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.red);
        // }
        // if (Input.GetKeyUp(KeyCode.Alpha3))
        // {
        //     //Get the Renderer component from the Buttom0
        //     var ButtomRenderer = Buttom3.GetComponent<Renderer>();
            
        //     //Call SetColor using the shader property name "_Color" and setting the color to red
        //     ButtomRenderer.material.SetColor("_Color", Color.white);
        // }
        
        
        Move();
    }

    void FixedUpdate()
    {
    }

    void Move()
    {
        transform.position = objectToFollow.position + center + offset;
        // Dampen towards the target rotation
        var rotation = objectToFollow.rotation * Quaternion.Euler(orientation);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation,  Time.deltaTime * 5.0f);
    }
}
