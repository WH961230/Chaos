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

    public float walkWeight;
    public float runWeight;
    public float backwardWeight;
    public float sideWeight;

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

        //get input
        float h = Input.GetAxis(HORIZONTAL);
        float v = Input.GetAxis(VERTICAL);
        bool isShift = Input.GetKey(KeyCode.LeftShift);

        if (h == 0) {
            SetAnimation(HORIZONTAL, 0);
        }

        if (h > 0) {
            SetAnimation(HORIZONTAL, sideWeight);
        }
        
        if (h < 0) {
            SetAnimation(HORIZONTAL, -sideWeight);
        }

        if (v == 0) {
            SetAnimation(VERTICAL, 0);
        }

        if (v > 0) {
            if (isShift) {
                SetAnimation(VERTICAL, runWeight);
            } else {
                SetAnimation(VERTICAL, walkWeight);
            }
        }
        
        if (v < 0) {
            SetAnimation(VERTICAL, backwardWeight);
        }

        if (h != 0 || v != 0) {
            Transform farTarget = VirtualCameraTargetController.instance.FarTarget;
            Vector3 farPosition = new Vector3(farTarget.position.x, 0, farTarget.position.z);
            Vector3 selfPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 selfToFarDir = (farPosition - selfPosition).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(selfToFarDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.fixedDeltaTime * RotateSpeed);
        }
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