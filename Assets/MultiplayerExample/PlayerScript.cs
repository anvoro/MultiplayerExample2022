using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer2D : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject brows;

    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        value: Color.white,
        writePerm: NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (brows != null)
            brows.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        playerColor.OnValueChanged += OnColorChanged;
        OnColorChanged(Color.white, playerColor.Value);

        if (IsOwner)
        {
            var randomColor = new Color(Random.value, Random.value, Random.value);
            ChangeColorServerRpc(randomColor);
        }
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    [ServerRpc]
    private void ChangeColorServerRpc(Color newColor)
    {
        playerColor.Value = newColor;

        ColorChangedClientRpc(newColor);
    }

    [ClientRpc]
    private void ColorChangedClientRpc(Color newColor)
    {
        if (!IsOwner) return;

        Debug.Log($"Цвет успешно изменён на {newColor}");
    }

    [ServerRpc]
    private void PlayEffectServerRpc()
    {
        PlayEffectMulticastRpc();
    }

    [Rpc(SendTo.Everyone, Delivery = RpcDelivery.Unreliable)]
    private void PlayEffectMulticastRpc()
    {
        if (brows != null)
        {
            brows.SetActive(true);
            if (hideBrowsCoroutine != null)
                StopCoroutine(hideBrowsCoroutine);

            hideBrowsCoroutine = StartCoroutine(HideBrowsAfterDelay());
        }
    }

    private Coroutine hideBrowsCoroutine;

    private IEnumerator HideBrowsAfterDelay()
    {
        yield return new WaitForSeconds(2f); // здесь — время задержки в секундах
        if (brows != null)
            brows.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        input = input.normalized;
        transform.position += (Vector3)input * Time.deltaTime * 5f;
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            var randomColor = new Color(Random.value, Random.value, Random.value);
            ChangeColorServerRpc(randomColor);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayEffectServerRpc();
        }
    }
}