using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFuseCSharp;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Base controller class that provides common UI functionality for all GameFuse UI controllers
    /// </summary>
    public abstract class BaseGameFuseUIController
    {
        // References that will be set by the main controller
        protected UIDocument uiDocument;
        protected GameFuseConfig config;
        protected ScrollView logScrollView;
        protected VisualElement loadingOverlay;
        
        // Root element for this controller
        protected VisualElement rootElement;
        
        // Enum for message types in the log
        public enum LogType
        {
            Info,
            Success,
            Warning,
            Error
        }
        
        /// <summary>
        /// Initialize this controller with the required references
        /// </summary>
        public virtual void Initialize(UIDocument document, GameFuseConfig gameConfig, ScrollView logView, VisualElement overlay)
        {
            uiDocument = document;
            config = gameConfig;
            logScrollView = logView;
            loadingOverlay = overlay;
        }
        
        /// <summary>
        /// Sets the root element for this controller
        /// </summary>
        public virtual void SetRootElement(VisualElement element)
        {
            rootElement = element;
            InitializeUI();
            RegisterCallbacks();
        }
        
        /// <summary>
        /// Cleans up event handlers when the controller is disabled
        /// </summary>
        public virtual void Cleanup()
        {
            UnregisterCallbacks();
        }
        
        /// <summary>
        /// Initialize UI elements specific to this controller
        /// </summary>
        protected abstract void InitializeUI();
        
        /// <summary>
        /// Register UI callbacks specific to this controller
        /// </summary>
        protected abstract void RegisterCallbacks();
        
        /// <summary>
        /// Unregister UI callbacks specific to this controller
        /// </summary>
        protected abstract void UnregisterCallbacks();
        
        /// <summary>
        /// Executes an async operation with loading overlay
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> action)
        {
            // Show loading overlay
            if (loadingOverlay != null)
            {
                loadingOverlay.style.display = DisplayStyle.Flex;
            }
            
            try
            {
                // Execute the action
                await action();
            }
            catch (Exception ex)
            {
                // Log any exceptions
                LogMessage($"Error: {ex.Message}", LogType.Error);
                Debug.LogException(ex);
            }
            finally
            {
                // Hide loading overlay
                if (loadingOverlay != null)
                {
                    loadingOverlay.style.display = DisplayStyle.None;
                }
            }
        }
        
        /// <summary>
        /// Logs a message to the UI
        /// </summary>
        protected void LogMessage(string message, LogType type = LogType.Info)
        {
            if (logScrollView == null) return;
            
            // Create the message element
            var logElement = new VisualElement();
            logElement.AddToClassList("log-message");
            
            // Add type-specific class
            switch (type)
            {
                case LogType.Info:
                    logElement.AddToClassList("info");
                    break;
                case LogType.Success:
                    logElement.AddToClassList("success");
                    break;
                case LogType.Warning:
                    logElement.AddToClassList("warning");
                    break;
                case LogType.Error:
                    logElement.AddToClassList("error");
                    break;
            }
            
            // Add timestamp and message
            logElement.Add(new Label($"[{DateTime.Now:HH:mm:ss}] {message}"));
            
            // Add to log and scroll to bottom
            logScrollView.Add(logElement);
            logScrollView.scrollOffset = new Vector2(0, float.MaxValue);
        }
        
        /// <summary>
        /// Creates a standardized list item for displaying in scroll views
        /// </summary>
        protected VisualElement CreateListItem(string title, Dictionary<string, string> properties = null)
        {
            var item = new VisualElement();
            item.AddToClassList("list-item");
            
            // Add title
            var titleLabel = new Label(title);
            titleLabel.AddToClassList("list-item-title");
            item.Add(titleLabel);
            
            // Add properties
            if (properties != null && properties.Count > 0)
            {
                var propsContainer = new VisualElement();
                propsContainer.AddToClassList("list-item-properties");
                
                foreach (var prop in properties)
                {
                    var propElement = new VisualElement();
                    propElement.AddToClassList("list-item-property");
                    
                    var keyLabel = new Label(prop.Key + ":");
                    keyLabel.AddToClassList("list-item-property-key");
                    propElement.Add(keyLabel);
                    
                    var valueLabel = new Label(prop.Value);
                    valueLabel.AddToClassList("list-item-property-value");
                    propElement.Add(valueLabel);
                    
                    propsContainer.Add(propElement);
                }
                
                item.Add(propsContainer);
            }
            
            return item;
        }
        
        /// <summary>
        /// Clears all children from a scroll view
        /// </summary>
        protected void ClearScrollView(ScrollView scrollView)
        {
            if (scrollView == null) return;
            
            scrollView.Clear();
        }
    }
}