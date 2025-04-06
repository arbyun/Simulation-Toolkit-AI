using UnityEngine;

namespace Examples.Unity.Cosmetic
{
    public class CustomCursor: MonoBehaviour
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Texture2D cursorTextureOnClick;

        private void Start()
        {
            Cursor.SetCursor(cursorTexture, Vector3.zero, CursorMode.ForceSoftware);
        }
    }
}