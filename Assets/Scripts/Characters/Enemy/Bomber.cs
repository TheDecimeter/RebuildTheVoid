﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    public Grapple weapon;
    public float attackSpeed = 20f, bombSpeed = 1, cooldown = 2f;
    public int damage = 3;


    private Tile Target;

    private Vector3 attackVector, approachVector, referenceVector;
    private float attackDistance = 50, attackHeight = 4, attackPosition = 0;

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

        StartCoroutine(Advance());
    }

    private Vector3 GetAttackVector()
    {
        return Vector3.up;
    }

    private Vector3 GetApproachVector()
    {
        return Vector3.forward;
    }

    private IEnumerator Advance()
    {
        while (!attacking)
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
                transform.position = Target.PullPoint.transform.position + attackVector + approachVector * attackPosition;
            }
            yield return null;
        }
    }

    private IEnumerator AttackCo()
    {
        attacking = true;
        while(attacking)
        {
            yield return new WaitForSeconds(cooldown);
            Attack();
        }
    }

    private void Attack()
    {
        //print("Bomber: attack");
        StartCoroutine(TempPullAnimate(.3f));
    }

    private IEnumerator TempPullAnimate(float seconds)
    {
        float wait = seconds / 3;

        Vector3 target = Target.transform.position + Offset();

        weapon.PointAt(target + Offset());
        yield return new WaitForSeconds(wait);
        weapon.PointAt(target + Offset());
        yield return new WaitForSeconds(wait);
        weapon.PointAt(target + Offset());
        yield return new WaitForSeconds(wait);

        weapon.Retract();

        if (Target.TryKill(damage))
        {
            attacking = false;
            StartCoroutine(Leave());
        }
    }


    private IEnumerator Leave()
    {
        referenceVector = this.transform.position;
        weapon.Retract();

        while (!attacking)
        {
            if (attackPosition <= -attackDistance)
            {
                spawner.RemoveBomber(this);
            }
            else
            {
                attackPosition -= Time.deltaTime * attackSpeed;
                transform.position = referenceVector + approachVector * attackPosition;
            }
            yield return null;
        }
    }

    public bool TryKill(int damage)
    {
        health -= damage;
        if(health<=0)
        {
            spawner.RemoveBomber(this);
            return true;
        }
        return false;
    }

    private Vector3 Offset()
    {
        return new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    Advance();
    //}
}
