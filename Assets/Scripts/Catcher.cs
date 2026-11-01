using UnityEngine;
using UnityEngine.InputSystem;

public class Catcher : MonoBehaviour
{
    public float speed = 10f;
    Camera cam;
    float halfWidth = 0.5f;

    void Start()
    {
        cam = Camera.main;
        var col = GetComponent<Collider2D>();
        if (col) halfWidth = col.bounds.extents.x;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Vector3 pos = transform.position;

        var kb = Keyboard.current;
        if (kb != null)
        {
            float dir = 0f;
            if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) dir -= 1f;
            if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) dir += 1f;
            pos.x += dir * speed * Time.deltaTime;
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            float targetX = cam.ScreenToWorldPoint(mouse.position.ReadValue()).x;
            pos.x = Mathf.MoveTowards(pos.x, targetX, speed * 1.5f * Time.deltaTime);
        }

        float camHalf = cam.orthographicSize * cam.aspect;
        pos.x = Mathf.Clamp(pos.x, -camHalf + halfWidth, camHalf - halfWidth);
        transform.position = pos;
    }
}