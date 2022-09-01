using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject explosions; 
    
    bool canBeDestroyed = false;
    public int scoreValue =100;
 

    // Start is called before the first frame update
    void Start()
    {
        Level.instance.AddDestructable();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -2)
        {
            DestriyDestructable();
        }

        if (transform.position.x < 17f && !canBeDestroyed)
        {
            canBeDestroyed = true;
            Gun[] guns = transform.GetComponentsInChildren<Gun>();
            foreach(Gun gun in guns)
            {
                gun.isActive = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeDestroyed)
        {
            return;
        }
        Bullet bullet = collision.GetComponent<Bullet>(); //take bullets collision
        if(bullet != null) 
        {
            if (!bullet.isEnemyBullet)
            {
                Level.instance.AddScore(scoreValue);
                DestriyDestructable();
                Destroy(bullet.gameObject); //destroy bullet
            }
        }
    }

    void DestriyDestructable()
    {
        Instantiate(explosions, transform.position, Quaternion.identity);
        
        Level.instance.RemoveDestructable();
        Destroy(gameObject); //destroy this object(enemy)
    }
}
