using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 15;
    public float factor = 1;
    float timeElapsed;
    
    /// <summary>
    /// This method shows how fast the bullet moves
    /// </summary>
    void Awake()
    {
        speed = 15;
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed * factor;
    }

    // Update is called once per frame
    /// <summary>
    /// Exactly 5 seconds of the projectile not hitting anything, it will be destroyed
    /// </summary>

    void Update()
    {
        timeElapsed += Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, 0, 0);
        if (timeElapsed >= 5)
        {
            Destroy(this.gameObject);
            timeElapsed = 0;
        }




    }
    /// <summary>
    /// If the bullet hits the enemy,
    /// </summary>
    /// 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }

    }

}
