using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    public delegate void CollectedDelegate();
    public static event CollectedDelegate Collected;

    //only triggers with character layer with the help of collision matrix
    private void OnTriggerEnter(Collider other)
    {
        //call SizeUp method from Area Manager and  SizeUpOverlapSphere from MainCharacterController 
        if (Collected != null)
            Collected();
        Debug.Log(other.transform.root.name);
        Destroy(gameObject);
    }
}
