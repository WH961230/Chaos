using Mirror;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour {
    private NetworkIdentity net;
    void Start() {
        net = GetComponent<NetworkIdentity>();
    }

    void Update() {
        if (!net.isOwned) {
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x == 0 && z == 0) {
            Debug.Log("xz == 0");
            return;
        }

        Vector3 off = new Vector3(x, 0, z);
        transform.Translate(off * Time.deltaTime);
    }
}