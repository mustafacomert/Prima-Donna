using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    private Camera mainCamera;
    private int fovZoomOut = 90;
    private int fovDefault = 60;
    private int lerpSpeed = 50;

    private void OnEnable()
    {
        MainCharacterController.DidAttack -= ZoomOut;
        MainCharacterController.DidAttack += ZoomOut;

        MainCharacterController.AttackAnimFinished -= ResetZooming;
        MainCharacterController.AttackAnimFinished += ResetZooming;
    }

    private void OnDisable()
    {
        MainCharacterController.DidAttack -= ZoomOut;
        MainCharacterController.AttackAnimFinished -= ResetZooming;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

   
    private void ZoomOut()
    {
        float currentFov = mainCamera.fieldOfView;
        mainCamera.fieldOfView = Mathf.Lerp(currentFov, fovZoomOut, lerpSpeed * Time.deltaTime);
    }

    private void ResetZooming()
    {
        float currentFov = mainCamera.fieldOfView;
        mainCamera.fieldOfView = Mathf.Lerp(currentFov, fovDefault, lerpSpeed * Time.deltaTime);
    }
}
