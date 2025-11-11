using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Image image;

    public float health = 100f;
    public float max_health = 100f;
    IKillable parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>(true);
        image = images[1];
        parent = GetComponentInParent<IKillable>();
        gameObject.SetActive(false);
    }

    public void change_health(float amount)
    {
        gameObject.SetActive(true);
        health -= amount;
        image.fillAmount = health / max_health;
        if (health <= 0)
        {
            parent.OnDeath();
        }
    }

    public void set_hp(float amount)
    {
        health = amount;
        if (image)
        {
            image.fillAmount = health / max_health;
        }

    }

    public void set_max_hp(float amount)
    {
        max_health = amount;
        health = health > max_health ? max_health : health;
        if (image)
        {
            image.fillAmount = health / max_health;
        }
    }

}
