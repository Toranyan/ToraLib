using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDesctructible
{

    void TakeDamage(DamageInstance damage);
    void Destroy();

}
