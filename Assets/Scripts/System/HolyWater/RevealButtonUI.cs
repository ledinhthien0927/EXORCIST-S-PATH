using UnityEngine;
using UnityEngine.UI;

public class RevealButtonUI : MonoBehaviour
{
    public static Button Instance;

    private void Awake()
    {
        Instance = GetComponent<Button>();
        gameObject.SetActive(false);
    }
}