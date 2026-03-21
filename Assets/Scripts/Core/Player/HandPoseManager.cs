using UnityEngine;

public class HandPoseManager : MonoBehaviour
{
    [System.Serializable]
    public class HandPoseEntry
    {
        public HandPoseType poseType;
        public GameObject handObject;
    }

    [SerializeField] private HandPoseEntry[] poses;

    public void SetPose(HandPoseType poseType)
    {
        if (poses == null) return;

        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i] == null || poses[i].handObject == null) continue;
            poses[i].handObject.SetActive(poses[i].poseType == poseType);
        }
    }

    public void ResetToDefault()
    {
        SetPose(HandPoseType.Default);
    }
}