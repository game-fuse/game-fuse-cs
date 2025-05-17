using UnityEngine;

namespace GameFuse.UIToolkit
{
    [CreateAssetMenu(fileName = "GameFuseConfig", menuName = "GameFuse/Config", order = 1)]
    public class GameFuseConfig : ScriptableObject
    {
        [Header("Game Settings")]
        [Tooltip("Your GameFuse Game ID")]
        [SerializeField] private string gameId;
        
        [Tooltip("Your GameFuse Game Token")]
        [SerializeField] private string gameToken;
        
        public string GameId
        {
            get => gameId;
            set => gameId = value;
        }
        
        public string GameToken
        {
            get => gameToken;
            set => gameToken = value;
        }
    }
}