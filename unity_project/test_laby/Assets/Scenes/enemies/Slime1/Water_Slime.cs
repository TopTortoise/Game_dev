using UnityEngine;

public class Water_Slime : IEnemy, IKillable
{
    private Health hp;
    public Rigidbody2D rb;
    private Animator anim;
    public float attack_radius = 1f;
    public Transform Attackpoint;
    public LayerMask enemy_layer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        speed = 2.5f;
        health = 3f;
        max_health = 3f;
        damage = 1f;
        anim = GetComponentInChildren<Animator>();
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody2D>();
        hp.set_max_hp(max_health);
        hp.set_hp(health);
        //caculate random postion 
        //
        anim.SetFloat("speed_y", -1);
        anim.SetBool("is_idle", true);
    }



    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);
        anim.SetBool("is_attacking", colliders.Length > 0);
        anim.SetBool("is_idle", colliders.Length == 0);
        anim.SetBool("following", colliders.Length == 0);
        if (getTarget() && colliders.Length == 0)
        {
            FollowTarget();
            anim.SetBool("following", true);
            anim.SetBool("is_idle", false);
        }
        else if (colliders.Length == 0)
        {
            anim.SetBool("following", false);
            anim.SetBool("is_idle", true);
            rb.linearVelocity = Vector2.zero;
        }else{

            anim.SetBool("following", false);
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("is_idle", false);
        }

    }

    public void Attack()
    {


        Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);

        foreach (Collider2D player in colliders)
        {
            player.GetComponent<IKillable>().hit(damage);
        }

        var state = anim.GetCurrentAnimatorStateInfo(0);
        state.ToString().StartsWith("attack_");
        anim.SetBool("is_attacking", false);


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Attackpoint.position, attack_radius);
    }


    public void FollowTarget()
    {

        Vector2 direction = ((Vector2)getTarget().position - rb.position).normalized;
        Debug.Log(direction);
        float speed_y = direction.y > 0 ? 1 : direction.y < 0 ? -1 : 0;
        float speed_x = direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0;
        anim.SetFloat("speed_y", direction.y);
        anim.SetFloat("speed_x", direction.x);
        rb.linearVelocity = direction * speed;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    }
    public void hit(float damage)
    {
        StartCoroutine(hit_toggle());
        hp.change_health(1);
    }

    public System.Collections.IEnumerator hit_toggle()
    {
        anim.SetBool("hit", true);
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("hit", false);


    }
    public void OnDeath()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }

}
