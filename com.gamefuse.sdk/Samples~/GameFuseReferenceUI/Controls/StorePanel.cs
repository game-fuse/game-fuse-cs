using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using GameFuse.Models.Shared;

namespace GameFuse.UI.Controls
{
    /// <summary>
    /// A custom control for displaying and managing the in-game store.
    /// Part of Phase 5: Store & Economy
    /// </summary>
    [UxmlElement]
    public partial class StorePanel : VisualElement
    {
        // Item details popup elements
        private VisualElement itemDetailsPopupOverlay;
        private VisualElement itemDetailsPopup;
        private Label itemDetailsName;
        private Label itemDetailsDescription;
        private Label itemDetailsCategory;
        private Label itemDetailsPrice;
        private VisualElement itemDetailsIcon;
        private Button itemDetailsPurchaseButton;
        
        // Constructor
        public StorePanel()
        {
            // Create the UI structure programmatically
            CreateStorePanelContent();
            
            // Create the item details popup
            CreateItemDetailsPopup();
            
            // Add the panel-content class for consistency with other panels
            AddToClassList("panel-content");
            
            // Log creation for debugging
            Debug.Log("StorePanel custom control created");
        }
        
        // Method to initialize the store panel elements after the controller attaches
        public void Initialize()
        {
            Debug.Log("StorePanel.Initialize() called");
        }
        
        private void CreateStorePanelContent()
        {
            // Header with title and status
            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("store-header");
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.justifyContent = Justify.SpaceBetween;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.marginBottom = 10;
            
            var titleLabel = new Label("Store");
            titleLabel.AddToClassList("panel-title");
            headerContainer.Add(titleLabel);
            
            var statusLabel = new Label();
            statusLabel.name = "store-status";
            statusLabel.AddToClassList("status-label");
            statusLabel.style.display = DisplayStyle.None;
            statusLabel.style.flexGrow = 1;
            statusLabel.style.marginLeft = 20;
            headerContainer.Add(statusLabel);
            
            Add(headerContainer);
            
            // Loading indicator
            var loadingIndicator = new VisualElement();
            loadingIndicator.name = "store-loading";
            loadingIndicator.AddToClassList("loading-indicator");
            var loadingText = new Label("Processing...");
            loadingText.AddToClassList("loading-text");
            loadingIndicator.Add(loadingText);
            loadingIndicator.style.display = DisplayStyle.None;
            Add(loadingIndicator);
            
            // Main container with tabs for different sections
            var mainContainer = new VisualElement();
            mainContainer.name = "store-main-container";
            mainContainer.style.flexGrow = 1;
            
            // Tabs container
            var tabsContainer = new VisualElement();
            tabsContainer.name = "store-tabs";
            tabsContainer.AddToClassList("tabs-container");
            tabsContainer.style.flexDirection = FlexDirection.Row;
            tabsContainer.style.marginBottom = 15;
            
            var availableTabButton = new Button();
            availableTabButton.name = "available-tab-button";
            availableTabButton.text = "Available Items";
            availableTabButton.AddToClassList("tab-button");
            availableTabButton.AddToClassList("active-tab");
            tabsContainer.Add(availableTabButton);
            
            var inventoryTabButton = new Button();
            inventoryTabButton.name = "inventory-tab-button";
            inventoryTabButton.text = "My Inventory";
            inventoryTabButton.AddToClassList("tab-button");
            tabsContainer.Add(inventoryTabButton);
            
            mainContainer.Add(tabsContainer);
            
            // Content container for tab panels
            var contentContainer = new VisualElement();
            contentContainer.name = "store-content-container";
            contentContainer.style.flexGrow = 1;
            
            // Create the different content panels
            CreateAvailableItemsPanel(contentContainer);
            CreateInventoryPanel(contentContainer);
            
            mainContainer.Add(contentContainer);
            Add(mainContainer);
        }
        
        private void CreateAvailableItemsPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "available-items-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.Flex; // Show by default
            
            var infoText = new Label("Browse and purchase items with your credits.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Controls container
            var controlsContainer = new VisualElement();
            controlsContainer.name = "available-controls";
            controlsContainer.AddToClassList("store-controls");
            controlsContainer.style.flexDirection = FlexDirection.Row;
            controlsContainer.style.justifyContent = Justify.SpaceBetween;
            controlsContainer.style.alignItems = Align.Center;
            controlsContainer.style.marginTop = 10;
            controlsContainer.style.marginBottom = 15;
            
            // Left side - category filter and search
            var leftControls = new VisualElement();
            leftControls.style.flexDirection = FlexDirection.Row;
            leftControls.style.alignItems = Align.Center;
            
            // Category filter dropdown (placeholder for now)
            var categoryContainer = new VisualElement();
            categoryContainer.style.flexDirection = FlexDirection.Row;
            categoryContainer.style.alignItems = Align.Center;
            categoryContainer.style.marginRight = 15;
            
            var categoryLabel = new Label("Category:");
            categoryLabel.AddToClassList("filter-label");
            categoryContainer.Add(categoryLabel);
            
            var categoryDropdown = new Button();
            categoryDropdown.name = "category-filter";
            categoryDropdown.text = "All Categories";
            categoryDropdown.AddToClassList("dropdown-button");
            categoryDropdown.style.marginLeft = 5;
            categoryContainer.Add(categoryDropdown);
            
            leftControls.Add(categoryContainer);
            
            // Search input
            var searchInput = new TextField();
            searchInput.name = "store-search-input";
            searchInput.AddToClassList("search-input");
            searchInput.textEdition.placeholder = "Search items...";
            searchInput.style.width = 200;
            leftControls.Add(searchInput);
            
            controlsContainer.Add(leftControls);
            
            // Right side - refresh button
            var rightControls = new VisualElement();
            rightControls.style.flexDirection = FlexDirection.Row;
            rightControls.style.alignItems = Align.Center;
            
            var creditsLabel = new Label("Credits: ");
            creditsLabel.AddToClassList("credits-label");
            rightControls.Add(creditsLabel);
            
            var creditsValue = new Label("0");
            creditsValue.name = "user-credits-display";
            creditsValue.AddToClassList("credits-value");
            rightControls.Add(creditsValue);
            
            var refreshButton = new Button();
            refreshButton.name = "available-refresh-button";
            refreshButton.text = "Refresh";
            refreshButton.AddToClassList("refresh-button");
            refreshButton.AddToClassList("secondary");
            refreshButton.style.marginLeft = 15;
            rightControls.Add(refreshButton);
            
            controlsContainer.Add(rightControls);
            
            panel.Add(controlsContainer);
            
            // Items grid container with scroll
            var scrollView = new ScrollView();
            scrollView.name = "available-items-scroll";
            scrollView.AddToClassList("items-scroll");
            scrollView.style.height = 350;
            scrollView.style.minHeight = 250;
            
            var itemsGrid = new VisualElement();
            itemsGrid.name = "available-items-grid";
            itemsGrid.AddToClassList("items-grid");
            
            // Empty state
            var emptyState = new VisualElement();
            emptyState.name = "available-empty-state";
            emptyState.AddToClassList("empty-state");
            
            var emptyIcon = new VisualElement();
            emptyIcon.AddToClassList("empty-icon");
            emptyState.Add(emptyIcon);
            
            var emptyText = new Label("No items available in the store.");
            emptyText.AddToClassList("empty-text");
            emptyState.Add(emptyText);
            
            itemsGrid.Add(emptyState);
            scrollView.Add(itemsGrid);
            panel.Add(scrollView);
            
            parent.Add(panel);
        }
        
        private void CreateInventoryPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.name = "inventory-panel";
            panel.AddToClassList("tab-panel");
            panel.style.display = DisplayStyle.None; // Hidden by default
            
            var infoText = new Label("Items you've purchased from the store.");
            infoText.AddToClassList("info-text");
            panel.Add(infoText);
            
            // Controls container
            var controlsContainer = new VisualElement();
            controlsContainer.name = "inventory-controls";
            controlsContainer.AddToClassList("store-controls");
            controlsContainer.style.flexDirection = FlexDirection.Row;
            controlsContainer.style.justifyContent = Justify.SpaceBetween;
            controlsContainer.style.alignItems = Align.Center;
            controlsContainer.style.marginTop = 10;
            controlsContainer.style.marginBottom = 15;
            
            // Left side - item count
            var countContainer = new VisualElement();
            countContainer.style.flexDirection = FlexDirection.Row;
            countContainer.style.alignItems = Align.Center;
            
            var countLabel = new Label("Items owned: ");
            countLabel.AddToClassList("count-label");
            var countValue = new Label("0");
            countValue.name = "inventory-count";
            countValue.AddToClassList("count-value");
            
            countContainer.Add(countLabel);
            countContainer.Add(countValue);
            controlsContainer.Add(countContainer);
            
            // Right side - refresh button
            var refreshButton = new Button();
            refreshButton.name = "inventory-refresh-button";
            refreshButton.text = "Refresh";
            refreshButton.AddToClassList("refresh-button");
            refreshButton.AddToClassList("secondary");
            controlsContainer.Add(refreshButton);
            
            panel.Add(controlsContainer);
            
            // Items grid container with scroll
            var scrollView = new ScrollView();
            scrollView.name = "inventory-items-scroll";
            scrollView.AddToClassList("items-scroll");
            scrollView.style.height = 350;
            scrollView.style.minHeight = 250;
            
            var itemsGrid = new VisualElement();
            itemsGrid.name = "inventory-items-grid";
            itemsGrid.AddToClassList("items-grid");
            
            // Empty state
            var emptyState = new VisualElement();
            emptyState.name = "inventory-empty-state";
            emptyState.AddToClassList("empty-state");
            
            var emptyIcon = new VisualElement();
            emptyIcon.AddToClassList("empty-icon");
            emptyState.Add(emptyIcon);
            
            var emptyText = new Label("You haven't purchased any items yet.");
            emptyText.AddToClassList("empty-text");
            emptyState.Add(emptyText);
            
            itemsGrid.Add(emptyState);
            scrollView.Add(itemsGrid);
            panel.Add(scrollView);
            
            parent.Add(panel);
        }
        
        private void CreateItemDetailsPopup()
        {
            // Create the item details popup overlay
            itemDetailsPopupOverlay = new VisualElement();
            itemDetailsPopupOverlay.name = "item-details-popup-overlay";
            itemDetailsPopupOverlay.AddToClassList("popup-overlay");
            itemDetailsPopupOverlay.pickingMode = PickingMode.Ignore;
            
            // Create the popup container
            itemDetailsPopup = new VisualElement();
            itemDetailsPopup.name = "item-details-popup";
            itemDetailsPopup.AddToClassList("item-popup-container");
            itemDetailsPopup.pickingMode = PickingMode.Position;
            
            // Popup header
            var header = new VisualElement();
            header.AddToClassList("popup-header");
            
            var title = new Label("Item Details");
            title.AddToClassList("popup-title");
            header.Add(title);
            
            var closeButton = new Button(() => HideItemDetailsPopup());
            closeButton.text = "×";
            closeButton.AddToClassList("popup-close");
            closeButton.AddToClassList("danger");
            closeButton.style.backgroundColor = new StyleColor(new Color(0.7f, 0.24f, 0.24f));
            closeButton.style.color = Color.white;
            closeButton.style.fontSize = 18;
            closeButton.style.unityFontStyleAndWeight = FontStyle.Bold;
            closeButton.style.borderLeftWidth = 0;
            closeButton.style.borderRightWidth = 0;
            closeButton.style.borderTopWidth = 0;
            closeButton.style.borderBottomWidth = 0;
            header.Add(closeButton);
            
            itemDetailsPopup.Add(header);
            
            // Popup content
            var content = new VisualElement();
            content.AddToClassList("popup-content");
            
            // Item display section
            var itemDisplay = new VisualElement();
            itemDisplay.AddToClassList("item-display");
            
            itemDetailsIcon = new VisualElement();
            itemDetailsIcon.name = "item-details-icon";
            itemDetailsIcon.AddToClassList("item-icon-large");
            itemDisplay.Add(itemDetailsIcon);
            
            var itemInfo = new VisualElement();
            itemInfo.AddToClassList("item-info");
            
            itemDetailsName = new Label();
            itemDetailsName.AddToClassList("item-name-large");
            itemInfo.Add(itemDetailsName);
            
            itemDetailsCategory = new Label();
            itemDetailsCategory.AddToClassList("item-category");
            itemInfo.Add(itemDetailsCategory);
            
            itemDetailsDescription = new Label();
            itemDetailsDescription.AddToClassList("item-description");
            itemInfo.Add(itemDetailsDescription);
            
            itemDisplay.Add(itemInfo);
            content.Add(itemDisplay);
            
            // Price and purchase section
            var purchaseSection = new VisualElement();
            purchaseSection.AddToClassList("purchase-section");
            
            var priceContainer = new VisualElement();
            priceContainer.AddToClassList("price-container");
            
            var priceLabel = new Label("Price:");
            priceLabel.AddToClassList("price-label");
            priceContainer.Add(priceLabel);
            
            itemDetailsPrice = new Label();
            itemDetailsPrice.AddToClassList("price-value");
            priceContainer.Add(itemDetailsPrice);
            
            purchaseSection.Add(priceContainer);
            
            itemDetailsPurchaseButton = new Button();
            itemDetailsPurchaseButton.name = "item-purchase-button";
            itemDetailsPurchaseButton.text = "Purchase";
            itemDetailsPurchaseButton.AddToClassList("purchase-button");
            itemDetailsPurchaseButton.AddToClassList("primary");
            purchaseSection.Add(itemDetailsPurchaseButton);
            
            content.Add(purchaseSection);
            
            itemDetailsPopup.Add(content);
            
            // Add the popup to the overlay
            itemDetailsPopupOverlay.Add(itemDetailsPopup);
            
            // Add the overlay to the panel
            Add(itemDetailsPopupOverlay);
        }
        
        public void ShowItemDetailsPopup(StoreItem item, int userCredits, Action<StoreItem> onPurchase)
        {
            // Set item data
            itemDetailsName.text = item.Name;
            itemDetailsDescription.text = item.Description ?? "No description available";
            itemDetailsCategory.text = item.Category ?? "Uncategorized";
            itemDetailsPrice.text = $"{item.Cost} Credits";
            
            // Update purchase button
            bool canAfford = userCredits >= item.Cost;
            itemDetailsPurchaseButton.SetEnabled(canAfford);
            itemDetailsPurchaseButton.text = canAfford ? "Purchase" : "Insufficient Credits";
            
            // Clear previous click handlers
            itemDetailsPurchaseButton.clickable = new Clickable(() => {
                if (canAfford)
                {
                    onPurchase?.Invoke(item);
                    HideItemDetailsPopup();
                }
            });
            
            // Show the popup
            itemDetailsPopupOverlay.style.display = DisplayStyle.Flex;
            
            // Register a click event on the overlay background to close the popup when clicking outside
            itemDetailsPopupOverlay.RegisterCallback<ClickEvent>(evt => {
                if (evt.target == itemDetailsPopupOverlay)
                {
                    HideItemDetailsPopup();
                }
            });
        }
        
        public void HideItemDetailsPopup()
        {
            itemDetailsPopupOverlay.style.display = DisplayStyle.None;
            
            // Unregister the click event to prevent memory leaks
            itemDetailsPopupOverlay.UnregisterCallback<ClickEvent>(evt => {
                if (evt.target == itemDetailsPopupOverlay)
                {
                    HideItemDetailsPopup();
                }
            });
        }
    }
}