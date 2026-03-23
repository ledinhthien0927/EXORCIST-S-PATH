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

            // Trigger the NEXT ritual to complete on F10 key press (Plays Sequence)
            if (Input.GetKeyDown(KeyCode.F10))
            {
                var rituals = FindObjectsByType<RitualManagerSimple>(FindObjectsSortMode.None);
                RitualManagerSimple nextRitual = null;

                foreach (var r in rituals)
                {
                    if (!r.IsDone)
                    {
                        nextRitual = r;
                        break;
                    }
                }
                
                if (nextRitual != null)
                {
                    Debug.Log($"[GameTestSystem] F10 Pressed - Forcing Ritual {nextRitual.name} to Complete!");
                    nextRitual.ForceComplete();
                }
                else
                {
                    Debug.LogWarning("[GameTestSystem] All rituals are already completed or none found!");
                }
            }
        }
    }
}
