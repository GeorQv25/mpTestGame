using Mirror;
using UnityEngine;


public class SyncObjectColor : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorUpdate))] private PlayerColor _objectColor;
    [SerializeField] private MeshRenderer _meshRenderer;


    public void UpdateColor(PlayerColor color)
    {
        _objectColor = color;
    }

    private void OnColorUpdate(PlayerColor oldValue, PlayerColor newValue)
    {
        if (GetPlatformMaterial(newValue, out Material material))
        {
            _meshRenderer.material = material;
        }
    }

    private bool GetPlatformMaterial(PlayerColor playerColor, out Material material)
    {
        material = Resources.Load<Material>($"PlayerMaterials/{playerColor}Material");
        return material != null;
    }
}