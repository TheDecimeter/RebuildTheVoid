using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    private Tile Target;

    private Vector3 attackVector, approachVector;
    private float attackPosition = 0, attackSpeed = 1f, attackDistance=10, attackHeight=10, bombSpeed=1;

    private const int MaxHealth = 10;
    private int health = MaxHealth;

    private bool attacking = false;
    private BomberSpawner spawner;


    public void Init(Tile target, BomberSpawner spawner)
    {
        this.Target = target;
        this.spawner = spawner;

        attackVector = GetAttackVector()*attackHeight;
        approachVector = GetApproachVector();

        attackPosition = attackDistance;
    }

    private Vector3 GetAttackVector()
    {
        return Vector3.up;
    }

    private Vector3 GetApproachVector()
    {
        return Vector3.forward;
    }

    private void Advance()
    {
        if (attackPosition <= 0)
        {
            attackPosition = 0;
            if (!attacking)
                StartCoroutine(AttackCo());
        }
        else
        {
            attackPosition -= Time.deltaTime * attackSpeed;
            transform.position = Target.transform.position + attackVector + approachVector * attackPosition;
        }
    }

    private IEnumerator AttackCo()
    {
        attacking = true;
        while(true){
            yield return new WaitForSeconds(bombSpeed);
            Attack();
        }
    }

    private void Attack()
    {
        print("Bomber: attack");
    }

    // Update is called once per frame
    void Update()
    {
        Advance();
    }
}
