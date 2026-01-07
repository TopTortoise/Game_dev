using UnityEngine;

public class EnemyController : IEnemy, IKillable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Health hp;
    public Rigidbody2D rb;
    private void Awake()
    {
        max_health = 3f;
        health = 3f;
        speed = 5.0f;
        rb = GetComponent<Rigidbody2D>();
        hp = gameObject.GetComponentInChildren<Health>();
        hp.set_max_hp(max_health);
        hp.set_hp(health);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (getTarget())
        {
            FollowTarget();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void FollowTarget()
    {

        Vector2 direction = ((Vector2)getTarget().position - rb.position).normalized;
        rb.linearVelocity = direction * speed;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);
    }
    public void hit(float damage)
    {

        hp.change_health(1);
    }
    public void OnDeath()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }
}
