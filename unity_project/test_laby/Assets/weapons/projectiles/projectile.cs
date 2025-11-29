using UnityEngine;

public class projectile : MonoBehaviour
{
    public float speed;
    public float damage;
    public float damage_mod;
    public float lifetime;
    public Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle+90);
        // transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg - 90);
        Invoke("DestroyProjectile", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("hit soemthing " + collider);
        IKillable obj = collider.gameObject.GetComponent<IKillable>();
        if (obj != null)
        {
            obj.hit(damage);
        }
        DestroyProjectile();
    }
}
