using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float fireRate = 1;
    public float CoolDown = .5f;
    public float damage = 3;
    public float range = 10;

    public SphereCollider rangeFinder;

    public Grapple weapon;

    private Stack<Bomber> enemies = new Stack<Bomber>();

    private void Start()
    {
        //rangeFinder.transform.localScale = new Vector3(range, range, range);
        rangeFinder.radius = range;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bomber b = other.gameObject.GetComponent<Bomber>();
        if (b)
        {
            enemies.Push(b);
            if (enemies.Count == 1)
                StartCoroutine(Shoot());
        }
    }
    
    private IEnumerator Shoot()
    {
        while (enemies.Count > 0)
        {
            Bomber b = enemies.Pop();
            do
            {
                if (b == null)
                    break;
                yield return new WaitForSeconds(CoolDown);
                if (b == null || Vector3.Distance(transform.position, b.transform.position) > range)
                    break;
                float pooptimer = 0;
                while (pooptimer < fireRate)
                {
                    if (b == null)
                        break;
                    weapon.PointAt(b.transform.position);
                    pooptimer += Time.deltaTime;
                    yield return null;
                }
                weapon.Retract();
            } while (!b.TryKill(Damage()));
        }
    }

    private int Damage()
    {
        if (damage >= 1)
            return (int)damage;
        if (Random.Range(0f, 1f) < damage)
            return 1;
        return 0;
    }
}
