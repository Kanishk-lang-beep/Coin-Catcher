using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    public float fallSpeed = 4f;
    public int points = 1;
    public float groundY = -3.5f; // where the coin hits the grass

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < groundY)
        {
            PixelFX.GroundPuff(new Vector2(transform.position.x, groundY - 0.4f));
            AudioManager.Play("sfx_miss", 0.8f);
            GameManager.Instance.LoseLife(); // missed coin
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Catcher>(out _))
        {
            PixelFX.CoinBurst(transform.position);
            AudioManager.Play("sfx_coin");
            GameManager.Instance.AddScore(points);
            Destroy(gameObject);
        }
    }
}
