using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuseCSharp;
using Boomlagoon.JSON;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Handles user management functionality including credits, scores, and attributes
    /// </summary>
    public class UserManagementController : BaseGameFuseUIController
    {
        // Credits UI
        private TextField creditsAmountField;
        private Button addCreditsButton;
        private Button setCreditsButton;
        
        // Score UI
        private TextField scoreAmountField;
        private Button addScoreButton;
        private Button setScoreButton;
        
        // Attributes UI
        private TextField attributeKeyField;
        private TextField attributeValueField;
        private Button setAttributeButton;
        private Button removeAttributeButton;
        private TextField multipleAttributesField;
        private Button setMultipleAttributesButton;
        private Button getAttributesButton;
        private ScrollView userAttributesScrollView;
        
        protected override void InitializeUI()
        {
            // Credits UI elements
            creditsAmountField = rootElement.Q<TextField>("credits-amount");
            addCreditsButton = rootElement.Q<Button>("add-credits-button");
            setCreditsButton = rootElement.Q<Button>("set-credits-button");
            
            // Score UI elements
            scoreAmountField = rootElement.Q<TextField>("score-amount");
            addScoreButton = rootElement.Q<Button>("add-score-button");
            setScoreButton = rootElement.Q<Button>("set-score-button");
            
            // Attributes UI elements
            attributeKeyField = rootElement.Q<TextField>("attribute-key");
            attributeValueField = rootElement.Q<TextField>("attribute-value");
            setAttributeButton = rootElement.Q<Button>("set-attribute-button");
            removeAttributeButton = rootElement.Q<Button>("remove-attribute-button");
            multipleAttributesField = rootElement.Q<TextField>("multiple-attributes");
            setMultipleAttributesButton = rootElement.Q<Button>("set-multiple-attributes-button");
            getAttributesButton = rootElement.Q<Button>("get-attributes-button");
            userAttributesScrollView = rootElement.Q<ScrollView>("user-attributes-scroll");
            
            // Add sample JSON to the multiple attributes field for better UX
            if (multipleAttributesField != null)
            {
                multipleAttributesField.value = "{\n  \"favorite_color\": \"blue\",\n  \"level\": \"10\",\n  \"experience\": \"1500\"\n}";
            }
        }
        
        protected override void RegisterCallbacks()
        {
            // Credits callbacks
            if (addCreditsButton != null)
            {
                addCreditsButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddCreditsClicked());
            }
            
            if (setCreditsButton != null)
            {
                setCreditsButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetCreditsClicked());
            }
            
            // Score callbacks
            if (addScoreButton != null)
            {
                addScoreButton.RegisterCallback<ClickEvent>(async (evt) => await OnAddScoreClicked());
            }
            
            if (setScoreButton != null)
            {
                setScoreButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetScoreClicked());
            }
            
            // Attributes callbacks
            if (setAttributeButton != null)
            {
                setAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetAttributeClicked());
            }
            
            if (removeAttributeButton != null)
            {
                removeAttributeButton.RegisterCallback<ClickEvent>(async (evt) => await OnRemoveAttributeClicked());
            }
            
            if (setMultipleAttributesButton != null)
            {
                setMultipleAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnSetMultipleAttributesClicked());
            }
            
            if (getAttributesButton != null)
            {
                getAttributesButton.RegisterCallback<ClickEvent>(async (evt) => await OnGetAttributesClicked());
            }
        }
        
        protected override void UnregisterCallbacks()
        {
            // Credits callbacks
            if (addCreditsButton != null)
            {
                addCreditsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnAddCreditsClicked());
            }
            
            if (setCreditsButton != null)
            {
                setCreditsButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSetCreditsClicked());
            }
            
            // Score callbacks
            if (addScoreButton != null)
            {
                addScoreButton.UnregisterCallback<ClickEvent>(async (evt) => await OnAddScoreClicked());
            }
            
            if (setScoreButton != null)
            {
                setScoreButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSetScoreClicked());
            }
            
            // Attributes callbacks
            if (setAttributeButton != null)
            {
                setAttributeButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSetAttributeClicked());
            }
            
            if (removeAttributeButton != null)
            {
                removeAttributeButton.UnregisterCallback<ClickEvent>(async (evt) => await OnRemoveAttributeClicked());
            }
            
            if (setMultipleAttributesButton != null)
            {
                setMultipleAttributesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnSetMultipleAttributesClicked());
            }
            
            if (getAttributesButton != null)
            {
                getAttributesButton.UnregisterCallback<ClickEvent>(async (evt) => await OnGetAttributesClicked());
            }
        }
        
        /// <summary>
        /// Adds credits to the current user
        /// </summary>
        private async Task OnAddCreditsClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage credits", LogType.Error);
                return;
            }
            
            if (!int.TryParse(creditsAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add credits to the user
                int credits = await GameFuseUser.CurrentUser.AddCreditsAsync(amount);
                
                LogMessage($"Added {amount} credits. New total: {credits}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Sets the credits for the current user
        /// </summary>
        private async Task OnSetCreditsClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage credits", LogType.Error);
                return;
            }
            
            if (!int.TryParse(creditsAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set user credits to specific amount
                int credits = await GameFuseUser.CurrentUser.SetCreditsAsync(amount);
                
                LogMessage($"Credits set to {credits}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Adds score to the current user
        /// </summary>
        private async Task OnAddScoreClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage score", LogType.Error);
                return;
            }
            
            if (!int.TryParse(scoreAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Add score to the user
                int score = await GameFuseUser.CurrentUser.AddScoreAsync(amount);
                
                LogMessage($"Added {amount} to score. New total: {score}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Sets the score for the current user
        /// </summary>
        private async Task OnSetScoreClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage score", LogType.Error);
                return;
            }
            
            if (!int.TryParse(scoreAmountField.value, out int amount))
            {
                LogMessage("Amount must be a valid integer", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set user score to specific amount
                int score = await GameFuseUser.CurrentUser.SetScoreAsync(amount);
                
                LogMessage($"Score set to {score}", LogType.Success);
            });
        }
        
        /// <summary>
        /// Sets a single attribute for the current user
        /// </summary>
        private async Task OnSetAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string key = attributeKeyField.value;
            string value = attributeValueField.value;
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Set a single attribute
                await GameFuseUser.CurrentUser.SetAttributeAsync(key, value);
                
                // Update UI
                await OnGetAttributesClicked();
                
                LogMessage($"Attribute '{key}' set successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Removes an attribute from the current user
        /// </summary>
        private async Task OnRemoveAttributeClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string key = attributeKeyField.value;
            
            if (string.IsNullOrEmpty(key))
            {
                LogMessage("Attribute key is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Remove the attribute
                await GameFuseUser.CurrentUser.RemoveAttributeAsync(key);
                
                // Update UI
                await OnGetAttributesClicked();
                
                LogMessage($"Attribute '{key}' removed successfully", LogType.Success);
            });
        }
        
        /// <summary>
        /// Sets multiple attributes for the current user from JSON
        /// </summary>
        private async Task OnSetMultipleAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to manage attributes", LogType.Error);
                return;
            }
            
            string jsonAttributes = multipleAttributesField.value;
            
            if (string.IsNullOrEmpty(jsonAttributes))
            {
                LogMessage("Attributes JSON is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                try
                {
                    // Parse the JSON into a dictionary
                    var jsonObject = JSONObject.Parse(jsonAttributes);
                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    
                    foreach (var key in jsonObject.GetKeys())
                    {
                        var value = jsonObject.GetValue(key);
                        if (value.Type == JSONValueType.String)
                        {
                            attributes[key] = value.Str;
                        }
                        else if (value.Type == JSONValueType.Number)
                        {
                            attributes[key] = value.Number.ToString();
                        }
                        else if (value.Type == JSONValueType.Boolean)
                        {
                            attributes[key] = value.Boolean.ToString();
                        }
                        else
                        {
                            attributes[key] = value.ToString();
                        }
                    }
                    
                    // Set multiple attributes
                    await GameFuseUser.CurrentUser.SetAttributesAsync(attributes);
                    
                    // Update UI
                    await OnGetAttributesClicked();
                    
                    LogMessage($"Set {attributes.Count} attributes successfully", LogType.Success);
                }
                catch (Exception ex)
                {
                    LogMessage($"Error parsing JSON: {ex.Message}", LogType.Error);
                }
            });
        }
        
        /// <summary>
        /// Gets and displays all attributes for the current user
        /// </summary>
        private async Task OnGetAttributesClicked()
        {
            if (GameFuseUser.CurrentUser == null || !GameFuseUser.CurrentUser.IsSignedIn())
            {
                LogMessage("You must be signed in to view attributes", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get user attributes
                var attributes = await GameFuseUser.CurrentUser.GetAttributesAsync();
                
                // Clear current attributes
                ClearScrollView(userAttributesScrollView);
                
                // Display attributes
                if (attributes != null && attributes.Count > 0)
                {
                    foreach (var attribute in attributes)
                    {
                        var properties = new Dictionary<string, string>
                        {
                            { "Value", attribute.Value }
                        };
                        
                        userAttributesScrollView.Add(CreateListItem(attribute.Key, properties));
                    }
                    
                    LogMessage($"Retrieved {attributes.Count} attributes", LogType.Success);
                }
                else
                {
                    LogMessage("No attributes found", LogType.Info);
                }
            });
        }
    }
}