using UnityEngine;

// Draws score, best score, lives (hearts) and the game-over screen with a pixel font.
// Needs PixelFont.png in Assets/Resources/. Put this on the GameManager object.
public class PixelHUD : MonoBehaviour
{
    const string Chars = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ:-!.^";
    const int Cols = 16, CellW = 8, CellH = 8;

    Texture2D font;
    bool warned;

    void Awake()
    {
        font = Resources.Load<Texture2D>("PixelFont");
        if (font) font.filterMode = FilterMode.Point;
    }

    int Scale { get { return Mathf.Max(2, Mathf.RoundToInt(Screen.height / 270f)); } }

    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        if (font == null)
        {
            if (!warned) { Debug.LogWarning("PixelHUD: put PixelFont.png in Assets/Resources/"); warned = true; }
            return;
        }
        var gm = GameManager.Instance;
        if (gm == null) return;

        int s = Scale;
        int m = 4 * s;

        // score + best (top left)
        Text("SCORE " + gm.score, m, m, s, Color.white);
        Text("BEST " + gm.Best, m, m + 9 * s, s, new Color(1f, .8f, .25f));

        // hearts (top right)
        int w = gm.maxLives * 7 * s - s;
        for (int i = 0; i < gm.maxLives; i++)
            Text("^", Screen.width - m - w + i * 7 * s, m, s,
                 i < gm.lives ? new Color(.9f, .2f, .25f) : new Color(.25f, .22f, .32f));

        if (gm.IsGameOver)
        {
            GUI.color = new Color(0.05f, 0.02f, 0.15f, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

            int big = s * 2;
            int cy = Screen.height / 2 - 30 * s;
            Center("GAME OVER", cy, big, new Color(.95f, .25f, .3f));
            Center("SCORE " + gm.score, cy + 20 * s, s * 2 - (s > 2 ? 1 : 0), Color.white);
            if (gm.score >= gm.Best && gm.score > 0)
                Center("NEW BEST!", cy + 32 * s, s, new Color(1f, .8f, .25f));
            else
                Center("BEST " + gm.Best, cy + 32 * s, s, new Color(1f, .8f, .25f));

            if ((int)(Time.unscaledTime * 2f) % 2 == 0)
                Center("CLICK OR PRESS R", cy + 48 * s, s, new Color(.7f, .9f, 1f));
        }
        GUI.color = Color.white;
    }

    int Width(string t, int s) { return t.Length * 6 * s - s; }

    void Center(string t, int y, int s, Color c)
    {
        Text(t, (Screen.width - Width(t, s)) / 2, y, s, c);
    }

    void Text(string t, int x, int y, int s, Color c)
    {
        DrawRun(t, x + s, y + s, s, new Color(0f, 0f, 0f, 0.6f * c.a)); // shadow
        DrawRun(t, x, y, s, c);
    }

    void DrawRun(string t, int x, int y, int s, Color c)
    {
        GUI.color = c;
        float tw = font.width, th = font.height;
        foreach (char raw in t.ToUpperInvariant())
        {
            int i = Chars.IndexOf(raw);
            if (i > 0)
            {
                int col = i % Cols, row = i / Cols;
                var uv = new Rect(col * CellW / tw, 1f - (row * CellH + 7) / th, 5f / tw, 7f / th);
                GUI.DrawTextureWithTexCoords(new Rect(x, y, 5 * s, 7 * s), font, uv);
            }
            x += 6 * s;
        }
    }
}
