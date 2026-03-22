using UnityEngine;
using ExorcistPath.Core.Managers;

namespace ExorcistPath.System
{
    public class GameTestSystem : MonoBehaviour
    {
        private void Update()
        {
            // Trigger WinGame on F11 key press
            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (GameManager.Instance != null)
                {
                    Debug.Log("[GameTestSystem] F11 Pressed - Triggering WinGame()");
                    GameManager.Instance.WinGame();
                }
                else
                {
                    Debug.LogWarning("[GameTestSystem] GameManager Instance not found!");
                }
            }
        }
    }
}
