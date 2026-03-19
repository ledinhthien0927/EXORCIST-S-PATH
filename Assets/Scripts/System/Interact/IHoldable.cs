using UnityEngine;

public interface IHoldable
{
    void OnPick(Transform holdPoint);
    void OnDrop(Vector3 worldPosition);
}