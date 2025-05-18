using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.Samples
{
    /// <summary>
    /// Main manager for the GameFuse Sample UI.
    /// Coordinates between the auth and data controllers.
    /// </summary>
    public class GameFuseSampleManager : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet _styleSheet;
        [SerializeField] private GameFuseAuthController _authController;
        [SerializeField] private GameFuseDataController _dataController;

        private void Awake()
        {
            // Apply style sheet
            var root = _document.rootVisualElement;
            root.styleSheets.Add(_styleSheet);
        }

        private void Start()
        {
            // Initialize the UI based on authentication state
            if (GameFuseUser.IsAuthenticated())
            {
                _authController.HideAuthPanel();
                _dataController.ShowDataPanel();
                _dataController.UpdateUserInfo();
            }
            else
            {
                _authController.ShowAuthPanel();
                _dataController.HideDataPanel();
            }
        }
    }
}