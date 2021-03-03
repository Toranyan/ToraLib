using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab;


    public void Fire() {

    }


    private void SpawnProjectile() {
        var proj = GameObject.Instantiate<Projectile>(projectilePrefab);
    }

}
