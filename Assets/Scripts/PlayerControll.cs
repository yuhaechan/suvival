using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerControll : MonoBehaviour
{
    private Rigidbody playerRigidbody;

    [SerializeField] private float walkSpeed; // walk speed;
    [SerializeField] private float runSpeed; // run speed;
    private float applySpeed; //now speed;

    [SerializeField] private float jumpForce; // Jump power;

    [SerializeField] private float crouchSpeed; // sit down speed;


    [SerializeField] private float lookSensitivity = 0.5f; // mouse Sensitivity
    [SerializeField] private float camerRotationLimit = 5f; // mouse up, down limit
    [SerializeField] private float currentCameraRotationX = 0f; // now camer Rotation x ( up, down )
    [SerializeField] private Camera theCamera; //player camer / next time i need 1 view 3 view
    private bool mouseOver = false; // mouse reverse 
    private int toggleView = 1; // now only 1

    private bool isRun = false; // if run
    private bool isGround = true; // if ground . 
    private CapsuleCollider capsuleCollider; // colider need to check ground

    private bool isCrounch = false; // if sit down
    [SerializeField] private float crouchPosY; // sit down hiehgt
    private float originPosY; // not sit down y
    private float applyCrouchPosy; // sit down y

    private void Start() // player move init
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerRigidbody = GetComponent<Rigidbody>();

        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosy = originPosY;
    }

    private void Update()
    {
        IsGround();
        TryJump();
        TryRun();

        TryCrouch();

        Move();

        CameraRotation();
        CharacterRotation();
        ChangeView();
    }

    private void TryCrouch() // try sit down
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crounch();
        }
    }

    private void Crounch() //sit down player
    {
        isCrounch = !isCrounch;

        IsGround(); // if player on the ground
        if(!isGround) // if player not on the ground 
        {
            return; // don't sit down
        }

        if(isCrounch)
        {
            applySpeed = crouchSpeed; 
            applyCrouchPosy = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosy = originPosY;
        }

        //theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosy, theCamera.transform.localPosition.z);
        StartCoroutine(CrounchCoroutine());
    }

    IEnumerator CrounchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;
        while(_posY != applyCrouchPosy)
        {
            count ++;
            _posY = Mathf.Lerp(_posY,applyCrouchPosy,0.3f);

            theCamera.transform.localPosition = new Vector3(0,_posY,0);
            if (count > 15)
            {
                break;
            }
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosy,0);
        
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+0.1f);
    }
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    private void Jump()
    {
        if(isCrounch)
        {
            Crounch();
        }
        playerRigidbody.velocity = transform.up * jumpForce;
    }
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        playerRigidbody.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation() // ����
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

    private void CharacterRotation() //�¿�
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
