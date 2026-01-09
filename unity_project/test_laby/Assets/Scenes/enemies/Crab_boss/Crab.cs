using UnityEngine;
using System.Collections;
public class Crab : IEnemy, IKillable
{

  private Health hp;
  public Rigidbody2D rb;
  public GameObject player;
  private void Awake()
  {

    max_health = 20f;
    health = 20f;
    speed = 5.0f;
    damage = 2;
    collision_damage = 3;
    rb = GetComponent<Rigidbody2D>();
    hp = gameObject.GetComponentInChildren<Health>();
    hp.set_max_hp(max_health);
    hp.set_hp(health);
    player = GameObject.FindWithTag("Player");
    Debug.Log("Player is " + player);
  }



  public float attack_cooldown = 2f;
  float time;
  bool isdashing = false;
  void Update()
  {
    time += Time.deltaTime;
    if (player != null && time > attack_cooldown && !isdashing)
    {
      StartCoroutine(Attack());
    }
  }

  public float dashSpeed = 20f;
  public float minDist = 2.1f;
  IEnumerator Attack()
  {
    isdashing = true;
    Debug.Log("Lets dashhh");
    //dash
    Vector2 playerPos = player.GetComponent<Rigidbody2D>().position;
    Vector2 direction = (playerPos - rb.position ).normalized;
    rb.linearVelocity = -direction * speed;
    yield return new WaitForSeconds(0.25f);
    is_collided = false;
    Debug.Log("direction is "+ direction);
    rb.linearVelocity = direction * dashSpeed;

    
    yield return new WaitUntil(() => Vector2.Distance(rb.position, playerPos+direction*2) <= minDist || is_collided);

    Debug.Log("Lets dash ended");
    rb.linearVelocity = Vector2.zero;
    isdashing = false;
    is_collided = false;
    time = 0;
  }
  bool is_collided = false;
   void OnCollisionEnter2D(Collision2D collision)
    {
      Debug.Log("hello there player");
      is_collided = true;
    }


  public void hit(float damage)
  {

    hp.change_health(damage);
  }


  public void OnDeath()
  {
    Debug.Log("Enemy Died");
    Destroy(gameObject);
  }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, minDist);
    }

}
