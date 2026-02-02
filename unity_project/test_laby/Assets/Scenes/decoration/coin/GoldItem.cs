using UnityEngine;

public class GoldItem : MonoBehaviour 
{
    public int goldValue = 10;
    public float pickupDistance = 3f; 
        public float moveSpeed = 10f;     
    private Transform playerTransform;
    private bool isFlying = false;

    void Start() {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update() {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

       
        if (distance < pickupDistance) {
            isFlying = true;
        }

        if (isFlying) {
            
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            CurrencyManager.Instance.AddGold(goldValue);
            Destroy(gameObject);
        }
    }
}