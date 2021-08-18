using System.Collections;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    private void OnEnable()
    {
        MainCharacterController.CharacterOnStage -= Kill;
        MainCharacterController.CharacterOnStage += Kill;
    }

    private void OnDisable()
    {
        MainCharacterController.CharacterOnStage -= Kill;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
       

    }

    private void FixedUpdate()
    {
    }
    private void Kill()
    {
        Debug.Log("killasdasd");
        animator.SetBool("isKilled", true);
        rb.AddForce(-transform.forward * 500);
    }
}
