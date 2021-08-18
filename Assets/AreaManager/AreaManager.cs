using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AreaManager : MonoBehaviour
{
    Image image;
    RectTransform imageTransform;

    bool canCallCoroutine = true;
    bool isAreaFilledCalled;
    //empty gameobject which is child of the character with trigger box collider 
    Transform directAttackRange;

    public delegate void AreaFilledDelegate();
    public static event AreaFilledDelegate AreaFilled;

    private void OnEnable()
    {
        //susbcribe to DidAttack event of the moveCharacterController script
        //Everytime player makes the area attack DidAttack function of that script will be called
        MainCharacterController.DidAttack -= CharacterDidAttack;
        MainCharacterController.DidAttack += CharacterDidAttack;
        //size up event
        Collectables.Collected -= SizeUp;
        Collectables.Collected += SizeUp;
        //disable
        FinishLine.FinishLinePassed -= DisableArea;
        FinishLine.FinishLinePassed += DisableArea;

    }

    private void OnDisable()
    {
        MainCharacterController.DidAttack -= CharacterDidAttack;
        //size up
        Collectables.Collected -= SizeUp;
        FinishLine.FinishLinePassed -= DisableArea;

    }

    private void Awake()
    {
        image = GetComponent<Image>();
        //reset fill amount on awake
        image.fillAmount = 0;
        //used for direct attacks
        directAttackRange = GameObject.FindGameObjectWithTag("DirectAttackRange").transform;
        directAttackRange.gameObject.SetActive(false);
        Vector3 scale = directAttackRange.localScale;
        directAttackRange.localScale = new Vector3(0, scale.y, scale.z);
        imageTransform = image.GetComponent<RectTransform>();
    }

    private void Update()
    {
        //current fill amount property of the  radial image
        float fillAmount = image.fillAmount;
        //if fill amount is less than 1 continue filling the image
        if (fillAmount < 1 && canCallCoroutine)
        {
            StartCoroutine("fillArea");
        }
        if(!isAreaFilledCalled && fillAmount > 0.5f)
        {
            AreaFilled();
            //prevent unneccessary calls to AreaFilled function
            //one call is enough for one state change
            isAreaFilledCalled = true;
        }
    }

    private IEnumerator fillArea()
    {
        canCallCoroutine = false;
        //every 0.1 fill amount of the image-
        //needs to make 17 degree rotation on z-axis to keep orientation image with the character
        float rotationAngle = 17;
        yield return new WaitForSeconds(1);
        //Increase fill Amount 0.1 on every one second
        image.fillAmount += 0.1f;
        //rotate to keep orientation 
        float zRotation = imageTransform.eulerAngles.z;
        imageTransform.eulerAngles += rotationAngle * Vector3.forward;
        //enlarge direct attack range on x axis, 1 unit corresponds to 0.1 radial fill of the image-
        //as per my calculations
        directAttackRange.gameObject.SetActive(true);
        if(directAttackRange.localScale.x < 4)
            directAttackRange.localScale += Vector3.right;
        //coroutine finished its job, now this script can make preceeding call to this coroutine 
        canCallCoroutine = true;
    }

    private void CharacterDidAttack()
    {
        //reset radial image size
        imageTransform.localScale = Vector3.one;
        //reset attack range's rotation
        imageTransform.localEulerAngles = Vector3.zero;
        //reset attack range
        image.fillAmount = 0;
        //to prevent continuous calls to AreaFilled event and thus prevents CPU over-usage
        isAreaFilledCalled = false;
        //reset direct attack area
        Vector3 scale = directAttackRange.localScale;
        directAttackRange.localScale = new Vector3(0, scale.y, scale.z); 
        directAttackRange.gameObject.SetActive(false);
    }

    private void SizeUp()
    {
        if (imageTransform.localScale.x < 2.5f)
            imageTransform.localScale += new Vector3(0.25f, 0.25f, 0); 
    }

    private void DisableArea()
    {
        gameObject.SetActive(false);
    }
}
