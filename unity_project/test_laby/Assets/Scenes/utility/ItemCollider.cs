using UnityEngine;

public class ItemCollider : MonoBehaviour, IKillable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string itemName = "Chest";

    private Health hp;
    public int value = 1; 
    public float health = 3f;
    private float max_health = 3f;
    void Awake()
    {
        hp = gameObject.GetComponentInChildren<Health>();
        hp.set_max_hp(max_health);
        hp.set_hp(health);
    }
   
    
     void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);
    }

     public void hit(float damage){
      
          hp.change_health(1);
    }
    public void OnDeath(){
      Debug.Log("Chest Destroyed");
      Destroy(gameObject);
    }
}
