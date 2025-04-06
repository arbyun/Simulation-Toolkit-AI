using UnityEngine;
using UnityEngine.InputSystem;

namespace Examples.Unity.Cosmetic
{
    public class CustomCursor: MonoBehaviour
    {
        [SerializeField] private InputActionReference clickAction;
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Texture2D cursorTextureOnClick;

        private void Start()
        {
            Cursor.SetCursor(cursorTexture, Vector3.zero, CursorMode.ForceSoftware);
            
            clickAction.action.performed += _ =>
                Cursor.SetCursor(cursorTextureOnClick, Vector3.zero, CursorMode.ForceSoftware);

            clickAction.action.canceled += _ =>
                Cursor.SetCursor(cursorTexture, Vector3.zero, CursorMode.ForceSoftware);
        }
    }
}