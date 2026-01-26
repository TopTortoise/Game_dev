using UnityEngine;
using System.Collections;
public class Crab :  IEnemy, IKillable
{

  private Health hp;
  public Rigidbody2D rb;
  public GameObject player;

  private SpriteRenderer spriteRend;

  private Renderer rend;
  private Color originalColor;

  public float flashTime = 0.3f;

  private Animator anim;
  private Vector2 moveDirection;
  
  private bool isDead;
  private void Awake()
  {

    max_health = 10f;
    health = 10f;
    speed = 5.0f;
    damage = 2;
    collision_damage = 3;
    rb = GetComponent<Rigidbody2D>();
    hp = gameObject.GetComponentInChildren<Health>();
    hp.set_max_hp(max_health);
    hp.set_hp(health);
    player = GameObject.FindWithTag("Player");
    rend = GetComponentInChildren<SpriteRenderer>();
    anim = GetComponentInChildren<Animator>();
    if (rend != null)
    {
      originalColor = rend.material.color;
    }
    else
    {
      Debug.LogError("No spriteRenderer found on Crab");
    }

    Debug.Log("Player is " + player);
    isDead = false;
  }



  public float attack_cooldown = 2f;
  float time;
  bool isdashing = false;
  void Update()
  {
    if (isDead) return;
    handleEffects();
    time += Time.deltaTime;
    if (player != null && time > attack_cooldown && !isdashing)
    {
      StartCoroutine(Attack());
    }
    
  }

  public float dashSpeed = 20f;
  public float minDist = 40f;
  IEnumerator Attack()
  {

    isdashing = true;
    Debug.Log("Lets dashhh");
    anim.SetBool("isDashing", true);
    //dash
    Vector2 playerPos = player.GetComponent<Rigidbody2D>().position;
    Vector2 direction = (playerPos - rb.position).normalized;
    moveDirection = direction;
    rb.linearVelocity = -direction * speed;
    yield return new WaitForSeconds(0.25f);
    is_collided = false;
    Debug.Log("direction is " + direction);

    float maxDashDistance = 15f;
    Vector2 targetPoint = rb.position + (direction * maxDashDistance);

    rb.linearVelocity = direction * dashSpeed;


    yield return new WaitUntil(() => Vector2.Distance(rb.position, targetPoint) <= 0.5f || is_collided);

    Debug.Log("Lets dash ended");
    rb.linearVelocity = Vector2.zero;
    isdashing = false;
    anim.SetBool("isDashing", false);
    is_collided = false;
    time = 0;
  }
  bool is_collided = false;
  void OnCollisionEnter2D(Collision2D collision)
  {
    Debug.Log("hello there player");
    is_collided = true;

    if (isdashing && CameraShake.Instance != null)
    {
      CameraShake.Instance.Shake(0.3f, 0.1f);
    }

  }

  public override void hit(float damage)
  {

    if (rend) StartCoroutine(DoFlash());
    //DamagePopupGenerator.current.CreatePopup(transform.position, damage);
    hp.change_health(damage);
    if (hp.health <= 0) OnDeath();

  }
  private IEnumerator DoFlash()
  {
    rend.material.color = Color.red;
    yield return new WaitForSeconds(flashTime);
    rend.material.color = originalColor;
  }

/*
  void UpdateSpriteFacing()
  {
    if (!rend) return;

    if (moveDirection.x > 0.01f)
      rend.flipX = false;
    else if (moveDirection.x < -0.01f)
      rend.flipX = true;
  }
*/

  public override void OnDeath()
  {
    if (isDead) return;
    isDead = true;
    isdashing = false;
    anim.SetBool("isDead", true);
    Debug.Log("Boss Died");
    StartCoroutine(DeathRoutine(1.5f));
    GameState.Instance.nrBossesDefeated++;
  }

  IEnumerator DeathRoutine(float duration)
  {
    Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
    yield return new WaitForSeconds(duration);
    Debug.Log($"Ended at {Time.time}");
    Destroy(gameObject);
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, minDist);
  }

}
