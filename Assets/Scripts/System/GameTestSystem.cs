using UnityEngine;
using ExorcistPath.Core.Managers;
using ExorcistPath.System; // Adjust namespaces if RitualManagerSimple is elsewhere

namespace ExorcistPath.System
{
    public class GameTestSystem : MonoBehaviour
    {
        private void Update()
        {
            // Trigger WinGame on F11 key press (Instant Win Map without playing sequence)
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

            // Trigger all rituals to complete on F10 key press (Plays Sequence)
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Debug.Log("[GameTestSystem] F10 Pressed - Forcing All Rituals to Complete!");
                var rituals = FindObjectsByType<RitualManagerSimple>(FindObjectsSortMode.None);
                
                if (rituals.Length > 0)
                {
                    foreach (var ritual in rituals)
                    {
                        ritual.ForceComplete();
                    }
                }
                else
                {
                    Debug.LogWarning("[GameTestSystem] No RitualManagerSimple found in scene!");
                }
            }
        }
    }
}
