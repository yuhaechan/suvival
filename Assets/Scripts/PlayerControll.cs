using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerControll : MonoBehaviour
{
    private Rigidbody playerRigidbody;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float lookSensitivity = 0.5f; // 마우스 민감도
    [SerializeField] private float camerRotationLimit = 5f;
    [SerializeField] private float currentCameraRotationX = 0f;
    [SerializeField] private Camera theCamera; //해보고싶은거 시점변경
    private bool mouseOver = false;
    private int toggleView = 1;


    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        CameraRotation();
        CharacterRotation();
        ChangeView();
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        playerRigidbody.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation() // 상하
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        if (!mouseOver)
        {
            currentCameraRotationX -= _cameraRotationX;
        }
        else
        {
            currentCameraRotationX += _cameraRotationX;
        }

        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -camerRotationLimit, camerRotationLimit);

        
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        

    }

    private void CharacterRotation() //좌우
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;

        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void ChangeView()
    {
        if(Input.GetKeyDown("v"))
        {
            //toggleView--;
            ToggleView(toggleView);
        }
    }

    private void ToggleView(int toggleView)
    {
        switch (toggleView)
        {
            case 0:
                Debug.Log(toggleView);
                toggleView = 1;
                break;
            case 1:
                toggleView = 0;
                break;
        }
    }
}
