using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace GameFuse.UI.Controls
{
    /// <summary>
    /// A custom control for displaying and managing user profile information.
    /// </summary>
    [UxmlElement]
    public partial class ProfilePanel : VisualElement
    {
        // Constructor
        public ProfilePanel()
        {
            // Create the template from the embedded UXML
            CreateProfilePanelContent();
            
            // Add the panel-content class for consistency with other panels
            AddToClassList("panel-content");
            
            // Log creation for debugging
            Debug.Log("ProfilePanel custom control created");
        }
        
        private void CreateProfilePanelContent()
        {
            // Basic structure - recreate the structure from ProfilePanel.uxml directly
            
            // Header with title and status
            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("profile-header");
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.justifyContent = Justify.SpaceBetween;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.marginBottom = 10;
            
            var titleLabel = new Label("Profile");
            titleLabel.AddToClassList("panel-title");
            headerContainer.Add(titleLabel);
            
            var statusLabel = new Label();
            statusLabel.name = "profile-status";
            statusLabel.AddToClassList("status-label");
            statusLabel.style.display = DisplayStyle.None;
            statusLabel.style.flexGrow = 1;
            statusLabel.style.marginLeft = 20;
            headerContainer.Add(statusLabel);
            
            Add(headerContainer);
            
            // Loading indicator
            var loadingIndicator = new VisualElement();
            loadingIndicator.name = "profile-loading";
            loadingIndicator.AddToClassList("loading-indicator");
            var loadingText = new Label("Processing...");
            loadingText.AddToClassList("loading-text");
            loadingIndicator.Add(loadingText);
            loadingIndicator.style.display = DisplayStyle.None;
            Add(loadingIndicator);
            
            // Profile info section
            CreateProfileInfoSection();
            
            // Score and Credits management side by side
            var managementContainer = new VisualElement();
            managementContainer.style.flexDirection = FlexDirection.Row;
            managementContainer.style.marginBottom = 15;
            
            // Create score and credits management in the container
            CreateScoreManagementSection(managementContainer);
            CreateCreditsManagementSection(managementContainer);
            
            Add(managementContainer);
            
            // Attributes section with increased height
            CreateAttributesSection();
            
            // Batch attributes section is removed, giving more space to attributes section
        }

        // Method to initialize the profile panel elements after the controller attaches
        public void Initialize()
        {
            // This method will be called from the ProfilePanelController
            // No need to query elements here as the controller will handle that
            Debug.Log("ProfilePanel.Initialize() called");
        }
        
        private void CreateProfileInfoSection()
        {
            var section = new VisualElement();
            section.name = "profile-info-section";
            section.AddToClassList("profile-section");
            
            var sectionTitle = new Label("User Information");
            sectionTitle.AddToClassList("section-title");
            section.Add(sectionTitle);
            
            // Two-column container
            var infoContainer = new VisualElement();
            infoContainer.style.flexDirection = FlexDirection.Row;
            infoContainer.style.marginTop = 10;
            
            // Left column
            var leftColumn = new VisualElement();
            leftColumn.style.width = Length.Percent(50);
            leftColumn.style.paddingRight = 15;
            
            // Right column
            var rightColumn = new VisualElement();
            rightColumn.style.width = Length.Percent(50);
            rightColumn.style.paddingLeft = 15;
            
            // Add columns to container
            infoContainer.Add(leftColumn);
            infoContainer.Add(rightColumn);
            
            // Left column - User ID, Username, Email
            
            // User ID row
            var userIdRow = new VisualElement();
            userIdRow.AddToClassList("profile-info-row");
            var userIdLabel = new Label("User ID:");
            userIdLabel.AddToClassList("profile-label");
            var userIdValue = new Label("Loading...");
            userIdValue.name = "user-id";
            userIdValue.AddToClassList("profile-value");
            userIdRow.Add(userIdLabel);
            userIdRow.Add(userIdValue);
            leftColumn.Add(userIdRow);
            
            // Username row
            var usernameRow = new VisualElement();
            usernameRow.AddToClassList("profile-info-row");
            var usernameLabel = new Label("Username:");
            usernameLabel.AddToClassList("profile-label");
            var usernameValue = new Label("Loading...");
            usernameValue.name = "username";
            usernameValue.AddToClassList("profile-value");
            usernameRow.Add(usernameLabel);
            usernameRow.Add(usernameValue);
            leftColumn.Add(usernameRow);
            
            // Email row
            var emailRow = new VisualElement();
            emailRow.AddToClassList("profile-info-row");
            var emailLabel = new Label("Email:");
            emailLabel.AddToClassList("profile-label");
            var emailValue = new Label("Loading...");
            emailValue.name = "email";
            emailValue.AddToClassList("profile-value");
            emailRow.Add(emailLabel);
            emailRow.Add(emailValue);
            leftColumn.Add(emailRow);
            
            // Right column - Score, Credits, Last Login
            
            // Score row
            var scoreRow = new VisualElement();
            scoreRow.AddToClassList("profile-info-row");
            var scoreLabel = new Label("Score:");
            scoreLabel.AddToClassList("profile-label");
            var scoreValue = new Label("0");
            scoreValue.name = "score";
            scoreValue.AddToClassList("profile-value");
            scoreValue.AddToClassList("score-value");
            scoreRow.Add(scoreLabel);
            scoreRow.Add(scoreValue);
            rightColumn.Add(scoreRow);
            
            // Credits row
            var creditsRow = new VisualElement();
            creditsRow.AddToClassList("profile-info-row");
            var creditsLabel = new Label("Credits:");
            creditsLabel.AddToClassList("profile-label");
            var creditsValue = new Label("0");
            creditsValue.name = "credits";
            creditsValue.AddToClassList("profile-value");
            creditsValue.AddToClassList("credits-value");
            creditsRow.Add(creditsLabel);
            creditsRow.Add(creditsValue);
            rightColumn.Add(creditsRow);
            
            // Last Login row
            var lastLoginRow = new VisualElement();
            lastLoginRow.AddToClassList("profile-info-row");
            var lastLoginLabel = new Label("Last Login:");
            lastLoginLabel.AddToClassList("profile-label");
            var lastLoginValue = new Label("N/A");
            lastLoginValue.name = "last-login";
            lastLoginValue.AddToClassList("profile-value");
            lastLoginRow.Add(lastLoginLabel);
            lastLoginRow.Add(lastLoginValue);
            rightColumn.Add(lastLoginRow);
            
            // Note: Login Count is removed as requested
            
            section.Add(infoContainer);
            Add(section);
        }
        
        private void CreateScoreManagementSection(VisualElement parentContainer = null)
        {
            var section = new VisualElement();
            section.name = "score-management-section";
            section.AddToClassList("profile-section");
            section.style.width = Length.Percent(50);
            section.style.marginRight = 10;
            
            var sectionTitle = new Label("Score Management");
            sectionTitle.AddToClassList("section-title");
            section.Add(sectionTitle);
            
            var controls = new VisualElement();
            controls.AddToClassList("management-controls");
            
            var scoreInput = new TextField();
            scoreInput.name = "score-input";
            scoreInput.textEdition.placeholder = "Enter score amount";
            scoreInput.AddToClassList("management-input");
            controls.Add(scoreInput);
            
            var buttons = new VisualElement();
            buttons.name = "score-buttons";
            buttons.AddToClassList("management-buttons");
            
            var setScoreButton = new Button();
            setScoreButton.name = "set-score-button";
            setScoreButton.text = "Set Score";
            setScoreButton.AddToClassList("management-button");
            setScoreButton.AddToClassList("primary");
            buttons.Add(setScoreButton);
            
            var addScoreButton = new Button();
            addScoreButton.name = "add-score-button";
            addScoreButton.text = "Add Score";
            addScoreButton.AddToClassList("management-button");
            addScoreButton.AddToClassList("secondary");
            buttons.Add(addScoreButton);
            
            controls.Add(buttons);
            section.Add(controls);
            
            var helpText = new Label("Set an absolute score value or add to the current score");
            helpText.AddToClassList("help-text");
            section.Add(helpText);
            
            // If parent container is provided, add to it; otherwise add to this (the profile panel)
            if (parentContainer != null)
                parentContainer.Add(section);
            else
                Add(section);
        }
        
        private void CreateCreditsManagementSection(VisualElement parentContainer = null)
        {
            var section = new VisualElement();
            section.name = "credits-management-section";
            section.AddToClassList("profile-section");
            section.style.width = Length.Percent(50);
            section.style.marginLeft = 10;
            
            var sectionTitle = new Label("Credits Management");
            sectionTitle.AddToClassList("section-title");
            section.Add(sectionTitle);
            
            var controls = new VisualElement();
            controls.AddToClassList("management-controls");
            
            var creditsInput = new TextField();
            creditsInput.name = "credits-input";
            creditsInput.textEdition.placeholder = "Enter credits amount";
            creditsInput.AddToClassList("management-input");
            controls.Add(creditsInput);
            
            var buttons = new VisualElement();
            buttons.name = "credits-buttons";
            buttons.AddToClassList("management-buttons");
            
            var setCreditsButton = new Button();
            setCreditsButton.name = "set-credits-button";
            setCreditsButton.text = "Set Credits";
            setCreditsButton.AddToClassList("management-button");
            setCreditsButton.AddToClassList("primary");
            buttons.Add(setCreditsButton);
            
            var addCreditsButton = new Button();
            addCreditsButton.name = "add-credits-button";
            addCreditsButton.text = "Add Credits";
            addCreditsButton.AddToClassList("management-button");
            addCreditsButton.AddToClassList("secondary");
            buttons.Add(addCreditsButton);
            
            controls.Add(buttons);
            section.Add(controls);
            
            var helpText = new Label("Set absolute credits value or add to the current balance");
            helpText.AddToClassList("help-text");
            section.Add(helpText);
            
            // If parent container is provided, add to it; otherwise add to this (the profile panel)
            if (parentContainer != null)
                parentContainer.Add(section);
            else
                Add(section);
        }
        
        private void CreateAttributesSection()
        {
            var section = new VisualElement();
            section.name = "attributes-section";
            section.AddToClassList("profile-section");
            section.style.marginTop = 20;
            
            var sectionHeader = new VisualElement();
            sectionHeader.AddToClassList("section-header");
            
            var sectionTitle = new Label("User Attributes");
            sectionTitle.AddToClassList("section-title");
            sectionHeader.Add(sectionTitle);
            
            var refreshButton = new Button();
            refreshButton.name = "refresh-attributes-button";
            refreshButton.text = "Refresh";
            refreshButton.AddToClassList("refresh-button");
            sectionHeader.Add(refreshButton);
            
            section.Add(sectionHeader);
            
            // Add New Attribute controls
            var addControls = new VisualElement();
            addControls.AddToClassList("add-attribute-controls");
            
            var keyInput = new TextField();
            keyInput.name = "new-attribute-key";
            keyInput.textEdition.placeholder = "Attribute Key";
            keyInput.AddToClassList("attribute-input");
            addControls.Add(keyInput);
            
            var valueInput = new TextField();
            valueInput.name = "new-attribute-value";
            valueInput.textEdition.placeholder = "Attribute Value";
            valueInput.AddToClassList("attribute-input");
            addControls.Add(valueInput);
            
            var addButton = new Button();
            addButton.name = "add-attribute-button";
            addButton.text = "Add Attribute";
            addButton.AddToClassList("management-button");
            addButton.AddToClassList("primary");
            addControls.Add(addButton);
            
            section.Add(addControls);
            
            // Attributes List with significantly increased height
            var scrollView = new ScrollView();
            scrollView.AddToClassList("attributes-scroll");
            scrollView.style.height = 400;
            scrollView.style.minHeight = 300;
            
            var attributesList = new VisualElement();
            attributesList.name = "attributes-list";
            attributesList.AddToClassList("attributes-list");
            scrollView.Add(attributesList);
            
            section.Add(scrollView);
            Add(section);
        }
        
    }
}