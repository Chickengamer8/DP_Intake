using UnityEngine;

public class boxGrabLayerManager : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask solidLayer;
    public LayerMask nonSolidLayer;
    public Transform player;
    public float teleportOffset = 1f;

    private bool playerInsideGrabZone = false;
    private GameObject boxObject;

    private void Start()
    {
        boxObject = transform.parent.gameObject;
        SetNonSolid();
    }

    private void Update()
    {
        if (!playerInsideGrabZone) return;

        if (Input.GetMouseButtonDown(1))
        {
            TeleportPlayerNextToBox();
            SetSolid();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SetNonSolid();
        }
    }

    private void TeleportPlayerNextToBox()
    {
        Vector3 boxPos = boxObject.transform.position;
        Vector3 playerPos = player.position;

        float direction = playerPos.x < boxPos.x ? -1f : 1f;
        Vector3 newPlayerPos = new Vector3(
            boxPos.x + direction * (boxObject.transform.localScale.x / 2f + teleportOffset),
            playerPos.y,
            playerPos.z
        );

        player.position = newPlayerPos;
    }

    private void SetSolid()
    {
        boxObject.layer = LayerMaskToLayer(solidLayer);
    }

    private void SetNonSolid()
    {
        boxObject.layer = LayerMaskToLayer(nonSolidLayer);
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int layerNumber = 0;
        int maskValue = mask.value;
        while (maskValue > 1)
        {
            maskValue = maskValue >> 1;
            layerNumber++;
        }
        return layerNumber;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideGrabZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideGrabZone = false;
        }
    }
}
