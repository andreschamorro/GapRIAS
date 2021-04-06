using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamLook : MonoBehaviour
{
    //Public variables
    public float minimumX = -60f;
    public float maximumX = 60f;
    public float minimumY = -360f;
    public float maximumY = 360f;

    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public Camera cam;
    public GameObject player;

    //Private variables
    float rotationY = 0f;
    float rotationX = 0f;

    Vector3 offset;
    public bool lockCursor = true;
    private bool m_cursorIsLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        // to lock the mouse cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        offset = cam.transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_cursorIsLocked)
        {
            rotationY += Input.GetAxis("Mouse X") * sensitivityY;
            rotationX += Input.GetAxis("Mouse Y") * sensitivityX;

            // Use Mathf.Clamp to constrain the desired rotation values to their maximum and minimum values
            rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

            // We can end this function by rotating the Player and Camera game objects accordingly
            transform.localEulerAngles = new Vector3(0, rotationY, 0);
            cam.transform.localEulerAngles = new Vector3(-rotationX, rotationY, 0);

            Camera.main.transform.position = player.transform.position + offset;
        }

        UpdateCursorLock();

    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
