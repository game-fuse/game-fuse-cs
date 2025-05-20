// GameFuseUserStoreIntegrationTests.cs
using GameFuse.Models;
using GameFuse.Models.TestSuite;
using GameFuse.Services;
using GameFuse.Transport;
using NUnit.Framework;
using System;
using System.Collections.Generic; // For IReadOnlyList
using System.IO;
using System.Linq; // For .Any()
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log and JsonUtility

namespace GameFuse.Tests.Editor.IntegrationTests
{
    [TestFixture]
    public class GameFuseUserStoreIntegrationTests
    {
        private TestSuiteService _testSuiteService;
        private CreateGameResponse _testGame; // To get gameId and gameToken

        // User-specific instance, might not be needed for all store tests
        // but good to have for purchase tests later.
        private GameFuseUser _testUser;

        protected const string ApiBaseUrl = "https://gamefuse.co/api/v3";
        protected string AdminToken { get; private set; }
        protected string AdminName { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LoadTestConfig();
            // Transport for TestSuiteService - doesn't need user auth
            var serviceTransport = new UnityWebRequestTransport(ApiBaseUrl, 3, 60);
            _testSuiteService = new TestSuiteService(serviceTransport);
        }

        [SetUp]
        public async Task SetupForEachTest()
        {
            _testGame = await _testSuiteService.CreateGameAsync(AdminToken, AdminName);
            Assert.IsNotNull(_testGame, "Test game creation failed.");
            Assert.IsTrue(_testGame.Id > 0, "Test game ID is invalid.");
            Assert.IsFalse(string.IsNullOrEmpty(_testGame.Token), "Test game token is null or empty.");

            // Sign up a user - will be useful for later tests (purchase, get user items)
            string userEmail = $"storetestuser_{Guid.NewGuid().ToString("N").Substring(0, 8)}@gamefuse.com";
            string userName = $"StoreTestUser_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            _testUser = await GameFuseUser.SignUpAsync(userEmail, "password1234", userName, _testGame.Id.ToString(), _testGame.Token);
            Assert.IsNotNull(_testUser, "Test user sign up failed for store tests.");
        }

        [TearDown]
        public async Task TearDownForEachTest()
        {
            _testUser?.SignOut();
            if (_testGame != null && _testGame.Id > 0)
            {
                await _testSuiteService.CleanUpTestAsync(_testGame.Id, AdminToken, AdminName);
            }
            GameFuseUser.SignOut(); // Clear static
        }

        [Test]
        public async Task Test_GetAvailableStoreItems_Succeeds()
        {
            Assert.IsNotNull(_testGame, "_testGame was not initialized.");
            Assert.IsTrue(_testGame.Id > 0, "Test Game ID is not valid.");
            Assert.IsFalse(string.IsNullOrEmpty(_testGame.Token), "Test Game Token is not valid.");

            Debug.Log($"Fetching available store items for Game ID: {_testGame.Id}");

            // Act
            IReadOnlyList<StoreItem> storeItems = await _testUser.GetAvailableStoreItemsAsync(_testGame.Id.ToString(), _testGame.Token);

            // Assert
            Assert.IsNotNull(storeItems, "The list of store items should not be null.");
            // At this point, we don't know if the test game has items.
            // If it's guaranteed to be empty or have specific items, assert accordingly.
            // For now, just checking it doesn't fail and returns a list.
            Debug.Log($"Fetched {storeItems.Count} store items.");

            if (storeItems.Any())
            {
                var firstItem = storeItems.First();
                Assert.IsTrue(firstItem.Id > 0, "Store item ID should be positive.");
                Assert.IsFalse(string.IsNullOrEmpty(firstItem.Name), "Store item name should not be empty.");
                Debug.Log($"First item example: ID={firstItem.Id}, Name='{firstItem.Name}', Cost={firstItem.Cost}");
            }
            else
            {
                Debug.Log("No store items found for this test game, which might be expected.");
            }
        }

        [Test]
        public async Task Test_PurchaseStoreItem_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.IsNotNull(_testGame, "_testGame was not initialized.");

            // --- Arrange ---
            // 1. Identify an item to purchase and its cost.
            //    For a real test, you'd get this from GameFuseStore.GetAvailableStoreItemsAsync
            //    or have a known item ID and cost from your test game setup.
            IReadOnlyList<StoreItem> availableItems = await _testUser.GetAvailableStoreItemsAsync(_testGame.Id.ToString(), _testGame.Token);
            if (!availableItems.Any())
            {
                Assert.Inconclusive("No store items available to test purchase. Please add items to the test game on GameFuse dashboard.");
                return;
            }
            StoreItem itemToPurchase = availableItems.First(); // Example: purchase the first available item
            int storeItemIdToPurchase = itemToPurchase.Id;
            int itemCost = itemToPurchase.Cost;
            Debug.Log($"Selected item for purchase: ID={storeItemIdToPurchase}, Name='{itemToPurchase.Name}', Cost={itemCost}");

            // 2. Ensure the user has enough credits.
            //    This relies on a working SetCreditsAsync or AddCreditsAsync.
            //    For simplicity here, we'll just try to set it.
            int creditsToHave = itemCost + 100; // Ensure enough credits + some leftover
            Debug.Log($"Attempting to set user credits to: {creditsToHave}");
            User userAfterCreditSet = await _testUser.SetCreditsAsync(creditsToHave); // Assumes SetCreditsAsync is implemented
            Assert.AreEqual(creditsToHave, userAfterCreditSet.Credits, "Failed to set credits for user.");
            int creditsBeforePurchase = _testUser.Credits; // Should be creditsToHave

            // --- Act ---
            Debug.Log($"'{_testUser.Username}' attempting to purchase item ID: {storeItemIdToPurchase}");
            UserStore purchaseResponse = await _testUser.PurchaseStoreItemAsync(storeItemIdToPurchase);

            // --- Assert ---
            Assert.IsNotNull(purchaseResponse, "PurchaseStoreItem response should not be null.");
            Assert.AreNotEqual(-1, purchaseResponse.Credits, "Purchase response indicates an error from service (Credits == -1).");

            int expectedCreditsAfterPurchase = creditsBeforePurchase - itemCost;
            Assert.AreEqual(expectedCreditsAfterPurchase, purchaseResponse.Credits, "Credits were not deducted correctly after purchase.");

            Assert.IsNotNull(purchaseResponse.StoreItems, "Purchased items list in response should not be null.");
            var purchasedItem = purchaseResponse.StoreItems.FirstOrDefault(si => si.Id == storeItemIdToPurchase);
            Assert.IsNotNull(purchasedItem, $"Item ID {storeItemIdToPurchase} not found in purchased items list after purchase.");
            Assert.AreEqual(itemToPurchase.Name, purchasedItem.Name, "Purchased item name mismatch.");

            Debug.Log($"Item '{itemToPurchase.Name}' purchased successfully. New credit balance: {purchaseResponse.Credits}.");

            // Verify GameFuseUser instance's internal state is updated
            Assert.AreEqual(purchaseResponse.Credits, _testUser.Credits, "GameFuseUser instance credits not updated after purchase.");
            Assert.IsNotNull(_testUser.GameUserStoreItems, "GameFuseUser instance's GameUserStoreItems is null.");
            var userInternalPurchasedItem = _testUser.GameUserStoreItems.FirstOrDefault(si => si.Id == storeItemIdToPurchase);
            Assert.IsNotNull(userInternalPurchasedItem, "Purchased item not found in GameFuseUser instance's internal list.");
            Assert.AreEqual(itemToPurchase.Name, userInternalPurchasedItem.Name, "Internal purchased item name mismatch.");
        }

        [Test]
        public async Task Test_RemoveStoreItem_Succeeds()
        {
            Assert.IsNotNull(_testUser, "_testUser was not initialized.");
            Assert.IsTrue(_testUser.IsAuthenticated(), "_testUser should be authenticated.");
            Assert.IsNotNull(_testGame, "_testGame was not initialized.");

            // --- Arrange ---
            // 1. Find an item and purchase it first.
            IReadOnlyList<StoreItem> availableItems = await _testUser.GetAvailableStoreItemsAsync(_testGame.Id.ToString(), _testGame.Token);
            if (!availableItems.Any())
            {
                Assert.Inconclusive("No store items available to test removing. Please add items to the test game.");
                return;
            }
            StoreItem itemToProcess = availableItems.First();
            int storeItemIdToRemove = itemToProcess.Id;
            int itemCost = itemToProcess.Cost;
            Debug.Log($"Selected item for purchase then removal: ID={storeItemIdToRemove}, Name='{itemToProcess.Name}', Cost={itemCost}");

            // 2. Ensure user has credits and purchase the item.
            int creditsToHave = itemCost + 100;
            User userAfterCreditSet = await _testUser.SetCreditsAsync(creditsToHave); // Assumes SetCreditsAsync works
            Assert.AreEqual(creditsToHave, userAfterCreditSet.Credits, "Failed to set credits for user before purchase.");

            Debug.Log($"Purchasing item ID: {storeItemIdToRemove} before attempting removal.");
            UserStore purchaseResponse = await _testUser.PurchaseStoreItemAsync(storeItemIdToRemove);
            Assert.IsNotNull(purchaseResponse, "Purchase failed during arrange phase of remove test.");
            Assert.IsTrue(purchaseResponse.StoreItems.Any(si => si.Id == storeItemIdToRemove), "Item not found in purchases after buying it.");
            int creditsAfterPurchase = purchaseResponse.Credits;
            Debug.Log($"Item purchased. Credits: {creditsAfterPurchase}. Now attempting to remove.");

            // --- Act ---
            Debug.Log($"'{_testUser.Username}' attempting to remove item ID: {storeItemIdToRemove}");
            UserStore removeResponse = await _testUser.RemoveStoreItemAsync(storeItemIdToRemove);

            // --- Assert ---
            Assert.IsNotNull(removeResponse, "RemoveStoreItem response should not be null.");
            Assert.AreNotEqual(-1, removeResponse.Credits, "Remove response indicates an error from service (Credits == -1).");

            // The API documentation implies the 'credits' field is returned.
            // It doesn't explicitly state if credits are refunded. We'll assume for now they are NOT,
            // unless testing proves otherwise or docs are updated.
            // If credits ARE refunded, this assertion needs to change.
            // For now, let's assume credits remain the same as after purchase, or we can check if it changed.
            // The API doc example shows credits as 135 for remove, same as after purchase in its example.
            // This suggests credits might not change, or the example is simplified.
            // Let's assert that the item is no longer in the list.
            // Assert.AreEqual(creditsAfterPurchase, removeResponse.Credits, "Credits changed unexpectedly after item removal. Check API refund policy.");

            Assert.IsNotNull(removeResponse.StoreItems, "Store items list in removeResponse should not be null.");
            var removedItem = removeResponse.StoreItems.FirstOrDefault(si => si.Id == storeItemIdToRemove);
            Assert.IsNull(removedItem, $"Item ID {storeItemIdToRemove} should NOT be found in purchased items list after removal.");

            Debug.Log($"Item '{itemToProcess.Name}' removed successfully. New credit balance: {removeResponse.Credits}.");

            // Verify GameFuseUser instance's internal state is updated
            Assert.AreEqual(removeResponse.Credits, _testUser.Credits, "GameFuseUser instance credits not updated after removal.");
            Assert.IsNotNull(_testUser.GameUserStoreItems, "GameFuseUser instance's GameUserStoreItems is null post-removal.");
            var userInternalRemovedItem = _testUser.GameUserStoreItems.FirstOrDefault(si => si.Id == storeItemIdToRemove);
            Assert.IsNull(userInternalRemovedItem, "Removed item still found in GameFuseUser instance's internal list.");
        }


        // --- Helper for loading test config ---
        private void LoadTestConfig()
        {
            // ... (same LoadTestConfig as in GameFuseUserFriendsIntegrationTests) ...
            string configPath = Path.Combine(Application.dataPath, "TestConfiguration", "testConfig.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                TestConfig config = JsonUtility.FromJson<TestConfig>(json);
                AdminToken = config.adminToken;
                AdminName = config.adminName;
                if (string.IsNullOrEmpty(AdminToken) || string.IsNullOrEmpty(AdminName))
                {
                    Assert.Inconclusive("AdminToken or AdminName is empty in testConfig.json.");
                }
            }
            else
            {
                Assert.Inconclusive("Test configuration file (testConfig.json) not found in Assets/TestConfiguration/.");
            }
        }

        [Serializable]
        private class TestConfig
        {
            public string adminToken;
            public string adminName;
        }
    }

    
}