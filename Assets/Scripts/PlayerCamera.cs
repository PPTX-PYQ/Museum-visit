using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform playerTarget;
    public float mouseSensitivity = 2f;
    public float minYAngle = -20f;
    public float maxYAngle = 80f;
    public float targetHeight = 1.5f;
    
    private float mouseX;
    private float mouseY;
    public float currentDistance = 3f;

    public GameObject QuestionsCanvas;
    public GameObject DeepSeekCanvas;
    public GameObject DrawCanvas;
    private float zoomSpeed = 2f;
    private float minDistance = 1f;
    private float maxDistance = 5f;
    private CanvasShowArtMessage artUI; 

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        artUI = FindObjectOfType<CanvasShowArtMessage>();
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;
        
        HandleCursorLock();
        
        if (!Cursor.visible)
        {
            HandleMouseInput();
        }
        
        HandleClick();
        HandleZoom();
        UpdateCameraPosition();
    }
    
    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (artUI != null)
                {
                    if(hit.collider.gameObject.name == "WorldQuestionsCanvas")
                    {
                        QuestionsCanvas.SetActive(true);
                    }
                    else if(hit.collider.gameObject.name == "WorldDeepSeekCanvas")
                    {
                        DeepSeekCanvas.SetActive(true);
                    }
                    else if(hit.collider.gameObject.name == "WorldDrawCanvas")
                    {
                        DrawCanvas.SetActive(true);
                    }
                    else
                    {
                        artUI.ShowArtByName(hit.collider.gameObject.name + "Art");
                    }
                }
            }
        }
    }
    
    void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    
    void HandleMouseInput()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle);
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }
    
    void UpdateCameraPosition()
    {
        Vector3 targetPosition = playerTarget.position + Vector3.up * targetHeight;
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
        Vector3 desiredPosition = targetPosition + rotation * negDistance;
        
        transform.rotation = rotation;
        transform.position = desiredPosition;
    }
}
