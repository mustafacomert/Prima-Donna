using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform mainCharacter;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    NavMeshAgent nav;
    Rigidbody rbEnemy;
    float force = 7500f;

    public enum AttackType
    {
        direct,
        area
    }

    public AttackType attackType;

    private void Awake()
    {
        mainCharacter = GameObject.FindGameObjectWithTag("Character").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        nav = GetComponent<NavMeshAgent>();
        rbEnemy = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if(navMeshAgent.enabled)
            navMeshAgent.SetDestination(mainCharacter.position);
    }

    public void isAttacked(AttackType attackType, Vector3 explosionPos, float radius)
    {
        float explosionForce = force / 2;
        if (attackType == AttackType.area)
        {
            explosionForce = force;
        }
        Debug.Log("Attack Attack");
        gameObject.tag = "Untagged";
        animator.SetBool("isFly", true);
        nav.enabled = false;
        rbEnemy.isKinematic = false;
        rbEnemy.AddExplosionForce(explosionForce, explosionPos, radius, 1f);
    }

    public void isKilled()
    {
        gameObject.tag = "Untagged";
        animator.SetBool("isDead", true);
        navMeshAgent.enabled = false;
    }
}
