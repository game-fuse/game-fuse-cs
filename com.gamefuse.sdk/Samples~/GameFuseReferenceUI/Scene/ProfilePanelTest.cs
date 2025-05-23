using UnityEngine;
using UnityEngine.UIElements;
using GameFuse.UI.Controls;

namespace GameFuse.UI.Test
{
    /// <summary>
    /// Simple test script to test the ProfilePanel custom control.
    /// </summary>
    public class ProfilePanelTest : MonoBehaviour
    {
        private UIDocument document;
        private ProfilePanelController profileController;

        void OnEnable()
        {
            document = GetComponent<UIDocument>();
            if (document == null)
            {
                Debug.LogError("UIDocument component not found on GameObject.");
                return;
            }

            // Setup once the UI document is loaded
            document.rootVisualElement.schedule.Execute(() => {
                SetupUI();
            }).StartingIn(100); // Small delay to ensure UI is loaded
        }

        private void SetupUI()
        {
            var root = document.rootVisualElement;
            var profilePanel = root.Q<VisualElement>("profile-panel");
            
            if (profilePanel == null)
            {
                Debug.LogError("Profile panel element not found in the UI document.");
                return;
            }

            // Initialize the profile controller with the panel
            profileController = new ProfilePanelController(profilePanel);
            
            // Log success
            Debug.Log("ProfilePanel test initialized successfully.");
            
            // For testing purposes, we might want to output the type of the panel
            Debug.Log($"Profile panel type: {profilePanel.GetType().Name}");
            
            // Check if it's a custom control
            if (profilePanel is ProfilePanel)
            {
                Debug.Log("Using ProfilePanel custom control correctly!");
            }
            else
            {
                Debug.Log("Using standard VisualElement for profile panel.");
            }
        }
    }
}