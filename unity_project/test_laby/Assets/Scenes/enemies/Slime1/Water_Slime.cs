using UnityEngine;
using System.Collections;
public class Water_Slime : IEnemy, IKillable
{
    private Health hp;
    public Rigidbody2D rb;
    private Animator anim;
    public float attack_radius = 1f;
    public Transform Attackpoint;
    public LayerMask enemy_layer;

    private Vector2 lastDirection = Vector2.zero;

    private Renderer rend;
    private Color originalColor;
    public float flashTime = 0.3F;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        speed = 2.5f;
        health = 15f;
        max_health = 15f;
        damage = 1f;
        anim = GetComponentInChildren<Animator>();
        hp = GetComponentInChildren<Health>();
        rb = GetComponent<Rigidbody2D>();
        hp.set_max_hp(max_health);
        hp.set_hp(health);
        
        rend = GetComponentInChildren<SpriteRenderer>();
        if(rend != null)
        {
            originalColor = rend.material.color;
        }else 
        {
            Debug.LogError("No SpriteRenderer found on Slime!");
        }
        //caculate random postion 
        //
        anim.SetFloat("speed_y", -1);
        anim.SetBool("is_idle", true);
    }



    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);
        if (anim.GetBool("is_attacking"))
        {
            rb.linearVelocity = Vector2.zero;
            return; // skip the rest of Update
        }
        anim.SetBool("is_attacking", colliders.Length > 0);
        if (getTarget() && !anim.GetBool("is_attacking"))
        {
            FollowTarget();
            anim.SetBool("following", true);
            anim.SetBool("is_idle", false);
        }
        else if (colliders.Length == 0 && !anim.GetBool("is_attacking"))
        {
            anim.SetBool("following", true);
            anim.SetBool("is_idle", true);
            rb.linearVelocity = lastDirection * speed;
        }
        else
        {

            anim.SetBool("following", false);
            anim.SetBool("is_idle", false);
        }

    }

    public void Attack()
    {


        Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);
        Debug.Log(colliders.Length);
        foreach (Collider2D player in colliders)
        {
            Debug.Log("killing "+player);
            Debug.Log(player.gameObject);
            player.GetComponent<IKillable>().hit(damage);
        }
        StartCoroutine(WaitForAttackAnimation());

    }

    private IEnumerator WaitForAttackAnimation()
    {
        //wait until the attack animation finishes (normalizedTime >= 1)
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        // After the animation ends:
        anim.SetBool("is_attacking", false);
    }
    private IEnumerator DoFlash()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        rend.material.color = originalColor; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Attackpoint.position, attack_radius);
    }


    public void FollowTarget()
    {

        Vector2 direction = ((Vector2)getTarget().position - rb.position).normalized;
        lastDirection = direction;
        // Debug.Log(direction);
        anim.SetFloat("speed_y", direction.y);
        anim.SetFloat("speed_x", direction.x);
        rb.linearVelocity = direction * speed;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("im hiti " + collision.gameObject);
    }
    public void hit(float damage)
    {
        StartCoroutine(hit_toggle());
        hp.change_health(damage);

        if (rend) StartCoroutine(DoFlash());
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
        GetComponent<LootDropper>().DropLoot();
    }

}
