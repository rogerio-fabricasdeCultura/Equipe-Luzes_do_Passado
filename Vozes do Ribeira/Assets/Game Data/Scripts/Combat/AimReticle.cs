using UnityEngine;

[DisallowMultipleComponent]
public sealed class AimReticle : MonoBehaviour
{
    [SerializeField] private CameraAimProvider aim;

    // Minimal placeholder; can be replaced with a Canvas without changing combat.
    private void OnGUI()
    {
        if (aim == null || Time.timeScale <= 0f || Cursor.lockState != CursorLockMode.Locked) return;
        if (!aim.TryGetScreenPosition(out Vector2 position)) return;
        float x = position.x;
        float y = Screen.height - position.y;
        GUI.DrawTexture(new Rect(x - 5f, y - 1f, 10f, 2f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(x - 1f, y - 5f, 2f, 10f), Texture2D.whiteTexture);
    }
}
