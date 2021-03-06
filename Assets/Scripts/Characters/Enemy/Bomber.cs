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
    private float attackDistance = 80, attackHeight = 4, attackPosition = 0;

    public int MaxHealth = 10;
    private int health;

    private bool attacking = false;
    private BomberSpawner spawner;

    private static bool didWarningShot = false;


    public void Init(Tile target, BomberSpawner spawner)
    {
        health = MaxHealth;
        this.Target = target;
        this.spawner = spawner;

        attackDistance *= Random.Range(.9f, 1.4f);
        attackHeight *= Random.Range(.9f, 1.4f);
        attackVector = GetAttackVector()*attackHeight;
        approachVector = GetApproachVector();

        attackPosition = attackDistance;

        StartCoroutine(Advance());

        if(!didWarningShot)
            StartCoroutine(WarningShoot(.1f, .2f));
    }

    private Vector3 GetAttackVector()
    {
        return Vector3.up;
    }

    private Vector3 GetApproachVector()
    {
        return (Vector3.forward * Random.Range(.9f,1.2f)+Vector3.up * Random.Range(.5f, .7f)).normalized;
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
        StartCoroutine(Shoot(.3f));
    }

    private Vector3 ShootPoint()
    {
        Vector3 t = Target.transform.position + Offset();
        return new Vector3(t.x, t.y + (Target.StackSize+1.5f), t.z);
        //return Target.Floor.transform.position + Offset();
    }

    private IEnumerator Shoot(float seconds)
    {
        float wait = seconds / 3;

        Vector3 target = ShootPoint();

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
            StopAllCoroutines();
            StartCoroutine(Leave());
        }
    }

    private IEnumerator WarningShoot(float waitShots, float waitCooldown)
    {
        float timer;

        Vector3 target = ShootPoint();

        for(int i=0; i<3; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                timer = 0;
                Vector3 t = target + Offset();
                while (timer < waitShots)
                {
                    timer += Time.deltaTime;
                    if (Target.HasPlayer())
                    {
                        didWarningShot = true;
                        weapon.PointAt(t);
                    }
                    else
                        weapon.Retract();
                    yield return null;
                }
            }

            weapon.Retract();
            yield return new WaitForSeconds(waitCooldown);
        }

    }


    private IEnumerator Leave()
    {
        referenceVector = this.transform.position;
        weapon.Retract();
        approachVector = new Vector3(approachVector.x, -approachVector.y, approachVector.z);

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
