using UnityEngine;

public class EnemyController : MonoBehaviour, IKillable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 5.0f;
    private Transform target;
    private Health hp;
    public Rigidbody2D rb;
    public float health = 3f;
    public float max_health = 3f;
        private void Awake()
    {
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
       if(target){
         FollowTarget();
       }else{
        rb.linearVelocity = Vector2.zero;
       }
    }

    public void FollowTarget(){

      Vector2 direction = ((Vector2)target.position - rb.position).normalized;
      rb.linearVelocity = direction * speed;

    }

    public void SetTarget(Transform t){
      target = t;
      // hp.change_health(1);
    }
    public void ResetTarget(){
      target = null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);
    }
    public void hit(float damage){
      
          hp.change_health(1);
    }
    public void OnDeath(){
      Debug.Log("Enemy Died");
      Destroy(gameObject);
    }
}
