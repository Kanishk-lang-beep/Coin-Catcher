using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float groundY = -3.5f;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < groundY)
        {
            PixelFX.GroundPuff(new Vector2(transform.position.x, groundY - 0.4f));
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Catcher>(out _))
        {
            PixelFX.BombBurst(transform.position);
            AudioManager.Play("sfx_bomb", 1f, 0.03f);
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
        }
    }
}
