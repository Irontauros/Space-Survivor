using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hitbox"))
        {
            Enemy enemy = collision.transform.parent.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(10f);
            }
            Destroy(gameObject);
        }
        else if (collision.transform.parent.CompareTag("Level"))
        {
            Destroy(gameObject);
        }
    }
}
