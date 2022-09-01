using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour
{
    Vector2 initialPos;

    Gun[] guns;

    float moveSpeed = 3f;

    int hits = 3;
    bool invincible = false;
    float invincibleTimer = 0;
    float invincibleTime = 2;


    bool moveUp;
    bool moveDown;
    bool moveLeft;
    bool moveRight;
    bool speedUp; //optional (bonus) for speedy movement

    bool shoot;

    GameObject shield;
    int powerUpGunLevel = 0;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        initialPos = transform.position;
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

        shield = transform.Find("Shield").gameObject;
        DeactivateShield();

        guns = transform.GetComponentsInChildren<Gun>();

       foreach (Gun gun in guns)
        {
            gun.isActive = true;
            if(gun.powerUpLevelReaquirement != 0)
            {
                gun.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Keys init
        moveUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        moveDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        moveLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        speedUp = Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);

        shoot = Input.GetKeyDown(KeyCode.Space);
        if (shoot)
        {
            shoot = false;
            foreach(Gun gun in guns)
            {
                if (gun.gameObject.activeSelf)
                {
                    gun.Shoot();
                }
            }
        }

        if (invincible)
        {
            if (invincibleTimer >= invincibleTime)
            {
                invincibleTimer = 0;
                invincible = false;
                spriteRenderer.enabled = true;
            }
            else
            {
                invincibleTimer += Time.deltaTime;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
        }   
    }

    void FixedUpdate()
    {
        //movement code
        Vector2 pos = transform.position;

        float moveAmount = moveSpeed * Time.fixedDeltaTime;
        if (speedUp)
        {
            moveAmount *= 3;
        }
        Vector2 move = Vector2.zero;

        if (moveUp)
        {
            move.y += moveAmount;
        }
        if (moveDown)
        {
            move.y -= moveAmount;
        }
        if (moveLeft)
        {
            move.x -= moveAmount;
        }
        if (moveRight)
        {
            move.x += moveAmount;
        }

        //for diagonally movement will equal standart movement
        float moveMagnitude = Mathf.Sqrt(move.x * move.x + move.y * move.y);
        if (moveMagnitude > moveAmount)
        {
            float ratio = moveAmount / moveMagnitude;
            move *= ratio;
        }
        

        pos += move;

        //for ship cant out from screen
        if(pos.x <= 1f)
        {
            pos.x = 1f;
        }
        if (pos.x >= 16.30f)
        {
            pos.x = 16.30f;
        }
        if (pos.y <= 0.5f)
        {
            pos.y = 0.5f;
        }
        if (pos.y >= 9.35f)
        {
            pos.y = 9.35f;
        }


        transform.position = pos;
    }

    void ActivateShield()
    {
        shield.SetActive(true);
    }

    void DeactivateShield()
    {
        shield.SetActive(false);
    }

    bool HasShield()
    {
        return shield.activeSelf;
    }

    void AddGuns()
    {
        powerUpGunLevel++;
        foreach(Gun gun in guns)
        {
            if (gun.powerUpLevelReaquirement <= powerUpGunLevel)
            {
                gun.gameObject.SetActive(true);
            }
            else
            {
                gun.gameObject.SetActive(false);
            }
        }
    }

    void ResetShip()
    {
        transform.position = initialPos;
        DeactivateShield();
        powerUpGunLevel = -1;
        AddGuns();
        hits = 3;
        Level.instance.ResetLevel();         
    }

    void Hit(GameObject gameObjectHit)
    {
        if (HasShield())
        {
            DeactivateShield();
        }
        else
        {
            if (!invincible)
            {
                hits--;
                if(hits == 0)
                {      
                    ResetShip();
                }
                else
                {
                    invincible = true;
                }
                Destroy(gameObjectHit);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        Bullet bullet = collision.GetComponent<Bullet>();
        if(bullet != null)
        {
            if (bullet.isEnemyBullet)
            {
                
                Hit(bullet.gameObject);
            }
        }

        Destructible destructible = collision.GetComponent<Destructible>();
        if (destructible != null)
        {
            Hit(destructible.gameObject);
        }

        PowerUp powerUp = collision.GetComponent<PowerUp>();
        if (powerUp)
        {
            if (powerUp.activateShield)
            {
                ActivateShield();
            }
            if (powerUp.addGuns)
            {
                AddGuns();
            }
            if (powerUp.increaseSpeed)
            {
                StartCoroutine(SpeedBuff());
            }
            Level.instance.AddScore(powerUp.pointValue);
            Destroy(powerUp.gameObject);
        }
    }
    IEnumerator SpeedBuff()
    {
        moveSpeed = moveSpeed*4;
        yield return new WaitForSeconds(5f);
        moveSpeed = moveSpeed/4;
    }

}
