using UnityEngine;

public class FinishLine : MonoBehaviour
{
    //decibel bar, disabled at the game start, 
    [SerializeField] private GameObject decibelBar;
    public delegate void FinishLinePassedDelegate();
    public static event FinishLinePassedDelegate FinishLinePassed;


    //can only interact with character, because the collision matrix
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("finish " + other.name);
        //enable decibel bar
        decibelBar.SetActive(true);
        if (FinishLinePassed != null)
        {
            Debug.Log("fhisj line passed");
            FinishLinePassed();
        }
    }


}
