using UnityEngine;

using UnityEngine.Tilemaps;
using System.Collections;

public class Water_Slime : IEnemy
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
  public Vector2 goal;
  public Tilemap tilemap;
  public int radius = 15;
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

    tilemap = FindFirstObjectByType<Tilemap>();
    choose_new_goal();
    rend = GetComponentInChildren<SpriteRenderer>();

    if (rend != null)
    {
      originalColor = rend.material.color;
    }
    else
    {
      Debug.LogError("No SpriteRenderer found on Slime!");
    }
    //caculate random postion 
    //
    anim.SetFloat("speed_y", -1);
    anim.SetBool("is_idle", true);
  }



  // Update is called once per frame 
  float time = 0.0f;
  public float min_dist = 1.0f;
  public float wait_time = 3.0f;
  void Update()
  {
    handleEffects();
    if (health == 0) return;
    Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);// check if player is in attack range
    anim.SetBool("is_attacking", colliders.Length > 0);//attack if player is in range
    if (anim.GetBool("is_attacking"))//attack
    {
      anim.SetBool("following", false);
      anim.SetBool("is_moving", false);
      anim.SetBool("is_idle", false);
      rb.linearVelocity = Vector2.zero;
      return; // skip the rest of Update
    }
    if (getTarget() && !anim.GetBool("is_attacking"))//following
    {
      time = 0.0f;
      goal = rb.position;
      FollowTarget();
      anim.SetBool("following", true);
      anim.SetBool("is_moving", false);
      anim.SetBool("is_idle", false);
    }
    else if (getTarget() == null)
    {
      anim.SetBool("following", false);
      if (Vector2.Distance(rb.position, goal) <= min_dist)
      {
        anim.SetBool("is_idle", true);
        anim.SetBool("is_moving", false);
        time += Time.deltaTime;
        rb.linearVelocity = Vector2.zero;
        if (time > wait_time)
        {
          choose_new_goal();
          time = 0.0f;
        }
      }
      else
      {
        anim.SetBool("is_moving", true);
        anim.SetBool("is_idle", false);
        walk();
      }

    }
    else if (colliders.Length == 0 && !anim.GetBool("is_attacking"))//idle
    {
      anim.SetBool("following", false);
      anim.SetBool("is_moving", false);
      anim.SetBool("is_idle", true);
      rb.linearVelocity = Vector2.zero;
    }
    else
    {

      rb.linearVelocity = Vector2.zero;
      anim.SetBool("is_moving", false);
      anim.SetBool("following", false);
      anim.SetBool("is_idle", true);
    }

  }

  public void walk()
  {

    Vector2 dir = Vector2.zero;
    int dist_x = (int)Mathf.Abs(rb.position.x - goal.x);
    int dist_y = (int)Mathf.Abs(rb.position.y - goal.y);

    if (dist_x > dist_y)
    {

      dir.x = Mathf.Sign(goal.x - rb.position.x);

      //go down or depenign on goal.x 

    }
    else
    {
      dir.y = Mathf.Sign(goal.y - rb.position.y);
    }
    if (tilemap.GetTile(new Vector3Int((int)rb.position.x + (int)dir.x, (int)rb.position.y + (int)dir.y, 0)).name.EndsWith("0"))
    {
      choose_new_goal();
      return;

    }

    // Debug.Log("dir is " + dir);
    lastDirection = dir;
    // Debug.Log(direction);
    anim.SetFloat("speed_y", dir.y);
    anim.SetFloat("speed_x", dir.x);
    rb.linearVelocity = dir * speed;

  }
  public void choose_new_goal()
  {
    Vector2 new_goal = rb.position;
    float x, y;
    TileBase v;
    do
    {
      float angle = Random.Range(0f, Mathf.PI * 2f);
      x = rb.position.x + Mathf.Cos(angle) * radius;
      y = rb.position.y + Mathf.Sin(angle) * radius;

      v = tilemap.GetTile(new Vector3Int((int)x, (int)y, 0));

    } while (!(v != null && v.name == "Wall"));//hard coded bad practice

    goal = new Vector2(x, y);

  }




  public void Attack()
  {


    Collider2D[] colliders = Physics2D.OverlapCircleAll(Attackpoint.position, attack_radius, enemy_layer);
    Debug.Log(colliders.Length);
    foreach (Collider2D player in colliders)
    {
      Debug.Log("killing " + player);
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
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(Attackpoint.position, attack_radius);
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(goal, 0.3f); // draw a small sphere at goal
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
  public override void hit(float damage)
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
  public override void OnDeath()
  {

    Debug.Log("Enemy Died");
    GetComponent<LootDropper>().DropLoot();
    Destroy(gameObject);
  }

}
