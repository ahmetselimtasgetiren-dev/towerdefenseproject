using UnityEngine;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>Marks generated children so only their owning duplicator can reuse or remove them.</summary>
    [DisallowMultipleComponent]
    public sealed class FlatShadowChild : MonoBehaviour
    {
        [HideInInspector] public Component owner;
    }
}
