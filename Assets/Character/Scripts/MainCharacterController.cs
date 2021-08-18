using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : Character
{
    private Vector3 dir;
    [SerializeField] private float speed = 800;
    [SerializeField] private Transform stage;
    [SerializeField] private GameObject directParticle;
    [SerializeField] private GameObject areaParticle;

    private Animator animator;
    private Rigidbody rb;

    private MoveCommand moveCommand;
    private IdleCommand idleCommand;
    private DieCommand dieCommand;
    private AreaAttackCommand areaAttackCommand;
    private DirectAttackCommand directAttackCommand;

    private Command command;

    private bool canAreaAttack;
    private bool areaAttack;
    private bool directAttack;
    private bool attackAnimsFinished = true;

    private bool isDead;
    private bool isDieCalled;
    private bool movingStage;

    private Vector3 contactPoint;
    private HashSet<EnemyController> enemiesInArea; 
    private HashSet<BreakableWindow> windowsInArea;

    private float currOverlapSphereSize = 35f;
    private float defaultOverlapSphereSize = 35f;

    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject levelCompletedMenu;

    GameObject spectators;

    //event for attacking state of the character
    //AreaManager and ZoomManager classes will subcribe this event
    public delegate void DidAttackDelegate();
    public static event DidAttackDelegate DidAttack;
    //event for attack animaitons finished
    //ZoomManager class will subscribe this event
    public delegate void AttackAnimFinishedDelegate();
    public static event AttackAnimFinishedDelegate AttackAnimFinished;
    //Character on the stage
    public delegate void CharacterOnStageDelegate();
    public static event CharacterOnStageDelegate CharacterOnStage;

    private void OnEnable()
    {
        AreaManager.AreaFilled -= CanAreaAttack;
        AreaManager.AreaFilled += CanAreaAttack;

        //overlap sphere size up
        Collectables.Collected -= SizeUpOverlapSphere;
        Collectables.Collected += SizeUpOverlapSphere;
        //
        FinishLine.FinishLinePassed -= MoveToStage;
        FinishLine.FinishLinePassed += MoveToStage;

    }

    private void OnDisable()
    {
        AreaManager.AreaFilled -= CanAreaAttack;
        Collectables.Collected -= SizeUpOverlapSphere;
        FinishLine.FinishLinePassed -= MoveToStage;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        moveCommand = new MoveCommand();
        idleCommand = new IdleCommand();
        dieCommand = new DieCommand();
        directAttackCommand = new DirectAttackCommand();

        areaAttackCommand = new AreaAttackCommand();

        enemiesInArea = new HashSet<EnemyController>();
        windowsInArea = new HashSet<BreakableWindow>();

        spectators = GameObject.FindGameObjectWithTag("Spectators");
        spectators.SetActive(false);
    }


    private void Update()
    {
        //check user input for horizontal and back/forward movement
        MovemenentInputCheck();
        //check user input for attack key which is Space key
        AttackInputCheck();
    }


    private void FixedUpdate()
    {
        //if DirectAttack and AreaAttack animations didn't finish, block the FixedUpdate
        //else continue executing FixedUpdate
        if (attackAnimsFinished && !movingStage)
        {
            if (isDead && !isDieCalled)
            {
                command = dieCommand;
            }
            else if (areaAttack)
            {
                command = areaAttackCommand;
            }
            else if (directAttack)
            {
                command = directAttackCommand;
            }
            //If user press any direction on the keyboard
            else if (dir.magnitude >= 0.1f)
            {
                command = moveCommand;
            }
            else
            {
                command = idleCommand;
            }
            command.Execute(this);
        }
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        GameObject col = collision.gameObject;
        if (col.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<EnemyController>().isKilled();
            contactPoint = collision.GetContact(0).point;
            isDead = true;
        }
    }


    //Direct Attack
    private void OnTriggerEnter(Collider other)
{
        if(other.CompareTag("Enemy"))
        {
            enemiesInArea.Add(other.GetComponent<EnemyController>());
        }
        else if(other.CompareTag("Window"))
        {
            windowsInArea.Add(other.GetComponent<BreakableWindow>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInArea.Remove(other.GetComponent<EnemyController>());
        }
        else if (other.CompareTag("Window"))
        {
            windowsInArea.Remove(other.GetComponent<BreakableWindow>());
        }
    }

    private void MovemenentInputCheck()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        dir = new Vector3(horizontal, 0f, vertical);
    }

    private void AttackInputCheck()
    {
        bool spaced = Input.GetKeyDown(KeyCode.Space);
        if (spaced && canAreaAttack)
        {
            areaAttack = true;
            canAreaAttack = false;
        }
        else if(spaced)
        {
            directAttack = true;
        }
    }

    //event listener, AreaManager class notify this, when fill amount of attack range became 1
    private void CanAreaAttack()
    {
        canAreaAttack = true;
    }

    //Unity Animation Event, 
    //this will called by unity when the DirectAttack or AreaAttack Animations finished 
    private void AttackAnimsFinished()
    {
        directParticle.SetActive(false);
        areaParticle.SetActive(false);
        //reset overlapshere size
        currOverlapSphereSize = defaultOverlapSphereSize;
        attackAnimsFinished = true;
        //notify zoom manager to reset the camera zooming
        AttackAnimFinished();
    }

    //Unity Animation event
    //when character is dead, and finished dead animation, freeze the game
    private void DeadAnimFinished()
    {
        Time.timeScale = 0f;
    }
    private void MoveToStage()
    {
        spectators.SetActive(true);
        movingStage = true;
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.Euler(Vector3.zero);
        animator.SetBool("isRunning", true);
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        float time = 3;
        Vector3 startPos = transform.position;
        Vector3 endPos = stage.position;

        float i = 0.0f;
        float rate = 1.0f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
        animator.SetBool("isRunning", false);
        animator.SetTrigger("areaAttack");
        if (CharacterOnStage != null)
        {
            CharacterOnStage();
        }
        yield return new WaitForSeconds(2f);
        levelCompletedMenu.SetActive(true);
    }

    


    private void SizeUpOverlapSphere()
    {
        //hard coded for now
        if(currOverlapSphereSize < 92)
            currOverlapSphereSize += currOverlapSphereSize * 0.214f;
    }

    override
    public void Move()
    {
        animator.SetBool("isRunning", true);
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(targetAngle * Vector3.up);
        rb.rotation = Quaternion.Slerp(rb.rotation, rot, Time.fixedDeltaTime * 10f);
        rb.velocity = dir * speed * Time.fixedDeltaTime + rb.velocity.y * Vector3.up;
    }

    override
    public void Idle()
    {
        animator.SetBool("isRunning", false);
        rb.velocity = rb.velocity.y * Vector3.up;
    }

    override
    public void AreaAttack()
    {
        areaParticle.SetActive(true);
        attackAnimsFinished = false;
        rb.velocity = Vector3.zero;
        animator.SetTrigger("areaAttack");
        rb.velocity = Vector3.zero;
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Damageable");
        //area attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currOverlapSphereSize, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                //is an enemy inside the range, attack him
                hitCollider.GetComponent<EnemyController>().isAttacked(EnemyController.AttackType.area, transform.position, currOverlapSphereSize);
                Rigidbody rBody = hitCollider.attachedRigidbody;
            }
            else if(hitCollider.attachedRigidbody.CompareTag("Window"))
            {
                //if damageable windows inside the range, break it
                hitCollider.GetComponent<BreakableWindow>().breakWindow();
            }
        }
        areaAttack = false;
        if(DidAttack != null)
            DidAttack();
    }

    override
    public void DirectAttack()
    {
        directParticle.SetActive(true);
        attackAnimsFinished = false;
        rb.velocity = Vector3.zero;
        animator.SetTrigger("directAttack");
        foreach (EnemyController e in enemiesInArea)
        {
            e.isAttacked(EnemyController.AttackType.direct, transform.position, currOverlapSphereSize);
        }
        foreach (BreakableWindow w in windowsInArea)
        {
            w.breakWindow();
        }
        enemiesInArea.Clear();
        windowsInArea.Clear();
        directAttack = false;
        if(DidAttack != null)
            DidAttack();
    }


    override
    public void Die()
    {
        float rotY = transform.localEulerAngles.y;
        rotY = (rotY > 180) ? rotY - 360 : rotY;
        
        rb.velocity = Vector3.zero;
        bool facedForward =  (Mathf.Abs(rotY) < 90);
        bool hitBehind = (contactPoint.z < transform.position.z);
        if ((hitBehind && facedForward) || (!hitBehind && !facedForward))
            rb.rotation = Quaternion.Euler((rotY + 180) * Vector3.up);     
        animator.SetBool("isDead", true);
        isDieCalled = true;
        gameOverMenu.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.DrawWireSphere(transform.position, currOverlapSphereSize);
    } 
}
