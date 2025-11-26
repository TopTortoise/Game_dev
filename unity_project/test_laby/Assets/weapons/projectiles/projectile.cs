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
        Invoke("DestroyProjectile", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
