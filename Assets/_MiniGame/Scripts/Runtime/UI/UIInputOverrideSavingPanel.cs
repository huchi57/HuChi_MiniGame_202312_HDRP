using UnityEngine;
using UnityEngine.InputSystem;

namespace UrbanFox.MiniGame
{
    public class UIInputOverrideSavingPanel : MonoBehaviour
    {
        public void SaveOverrides()
        {
            SettingsManager.Instance.InputBindingsOverride = InputManager.Instance.InputActions.SaveBindingOverridesAsJson();
        }

        public void RemoveOverrides()
        {
            SettingsManager.Instance.InputBindingsOverride = string.Empty;
            InputManager.Instance.InputActions.RemoveAllBindingOverrides();
        }
    }
}
