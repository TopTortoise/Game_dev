using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyController : IEnemy, IKillable
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  private Health hp;
  public Rigidbody2D rb;
  public Tilemap tilemap;
  public Vector2 goal;
  private void Awake()
  {
    max_health = 3f;
    health = 3f;
    speed = 5.0f;
    rb = GetComponent<Rigidbody2D>();
    hp = gameObject.GetComponentInChildren<Health>();
    hp.set_max_hp(max_health);
    hp.set_hp(health);
    tilemap = FindFirstObjectByType<Tilemap>();
    Debug.Log("tilemap is" + tilemap);
    choose_new_goal();
  }

  void Start()
  {

  }

  // Update is called once per frame
  public bool is_walker = true;
  float time = 0.0f;
  void Update()
  {
    if (is_walker && Vector2.Distance(rb.position, goal) <= 1f)
    {
      time += Time.deltaTime;
      rb.linearVelocity = Vector2.zero;

      Debug.Log("choosing");
      if (time > 3.0f)
      {
        choose_new_goal();
        time = 0.0f;
      }
    }
    else if (is_walker && Vector2.Distance(rb.position, goal) > 1f)
    {
      walk();
    }
    // else if (getTarget())
    // {
    // FollowTarget();
    // }
    // else
    // {
    // rb.linearVelocity = Vector2.zero;
    // }
  }

  public int radius = 15;
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

      Debug.Log("Tilemap is " + tilemap);
      v = tilemap.GetTile(new Vector3Int((int)x, (int)y, 0));

      Debug.Log("v ends with 49? " + v.name.EndsWith("49"));
    } while (!v.name.EndsWith("49"));

    goal = new Vector2(x, y);

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

    Debug.Log("dir is " + dir);
    rb.linearVelocity = dir * speed;

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

    hp.change_health(damage);
  }
  public void OnDeath()
  {
    Debug.Log("Enemy Died");
    Destroy(gameObject);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(goal, 0.3f); // draw a small sphere at goal
  }
}
