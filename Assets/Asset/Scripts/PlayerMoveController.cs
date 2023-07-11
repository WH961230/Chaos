using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerMoveController : MonoBehaviour {
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    #region 组件

    private NetworkIdentity net;
    private Animator animator;
    private CharacterController controller;
    private RigBuilder rigBuilder;

    #endregion

    public float animParamSpeed;
    public float MoveSpeed;
    public float RotateSpeed;

    private RigLayer headRigLayer;

    void Start() {
        net = GetComponent<NetworkIdentity>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();

        for (int i = 0; i < rigBuilder.layers.Count; i++) {
            if (rigBuilder.layers[i].name.Equals("HeadRigLayer")) {
                headRigLayer = rigBuilder.layers[i];
                break;
            }
        }

        if (net.isOwned) {
            gameObject.tag = "Player";

            MultiAimConstraint aim = transform.GetComponentInChildren<MultiAimConstraint>();
            var datas = aim.data.sourceObjects;
            datas.Add(new WeightedTransform(VirtualCameraTargetController.instance.FarTarget, 1));
            aim.data.sourceObjects = datas;
            rigBuilder.Build();
        }
    }

    private void FixedUpdate() {
        if (!net.isOwned) {
            return;
        }

        float h = Input.GetAxis(HORIZONTAL);
        float v = Input.GetAxis(VERTICAL);
        bool isShift = Input.GetKey(KeyCode.LeftShift);
        Debug.DrawLine(transform.position, new Vector3(VirtualCameraTargetController.instance.FarTarget.position.x, 0, VirtualCameraTargetController.instance.FarTarget.position.z).normalized, Color.red);

        if (h == 0 && v <= 0) {
            animator.SetFloat(HORIZONTAL, 0);
            animator.SetFloat(VERTICAL, 0);
            return;
        }

        SetAnimation(HORIZONTAL, isShift ? 2 * h : h);
        SetAnimation(VERTICAL, isShift ? 2 * v : v);

        Transform farTarget = VirtualCameraTargetController.instance.FarTarget;
        Vector3 newForward = (new Vector3(farTarget.position.x, 0, farTarget.position.z) - transform.position)
            .normalized;
        transform.forward = Vector3.Lerp(transform.forward, newForward, Time.fixedDeltaTime * RotateSpeed);
        controller.Move(transform.forward * (isShift ? 2 * MoveSpeed : MoveSpeed) * Time.fixedDeltaTime);
    }

    private void SetAnimation(string animParam, float value) {
        float currentAnim = animator.GetFloat(animParam);
        if (Mathf.Abs(currentAnim - value) > .01f) {
            animator.SetFloat(animParam, Mathf.Lerp(currentAnim, value, Time.fixedDeltaTime * animParamSpeed));
        } else {
            animator.SetFloat(animParam, value);
        }
    }
}