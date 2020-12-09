using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public Rigidbody2D rb;
    

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.right * bulletSpeed;
    }

    //register bullet when it hit through an object
    void OnTriggerEnter2D(Collider2D collision)
    {
        SlimeControl slime = collision.GetComponent<SlimeControl>();
        if (collision.tag == "Enemy")
        {
            slime.TakeDamage();
            Debug.Log("it works");
        }
        //Destroy(gameObject);
    }           
}
