using UnityEngine;

//make camera follow the boy, with the offset which obtained by scene, before running the game
public class FollowerCamera : MonoBehaviour
{
    private Transform targetBoy;
    private Vector3 desiredPos;
    private float offsetZ;
    private float offsetX;
    private float smoothSpeed = 10f;
    private Vector3 smoothedPos;
    private void Awake()
    {
        desiredPos = transform.position;
        targetBoy = GameObject.FindGameObjectWithTag("Character").transform;
        offsetX = transform.position.x - targetBoy.position.x;
        offsetZ = transform.position.z - targetBoy.position.z;
        offsetX = Mathf.Abs(offsetX);
        offsetZ = Mathf.Abs(offsetZ);
    }
    private void LateUpdate()
    {
        desiredPos.x = targetBoy.position.x - offsetX;
            
        desiredPos.z = targetBoy.position.z - offsetZ;
        smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }


    private void StageAngle()
    {
        Vector3 stageOffset = new Vector3(10, 10, 10);

    }
}