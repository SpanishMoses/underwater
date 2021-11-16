using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGiver : MonoBehaviour
{

    public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(float amount){
        enemy.health -= amount;

        if (enemy.health <= 0){
            Destroy(enemy.whole);
        }
    }
}
