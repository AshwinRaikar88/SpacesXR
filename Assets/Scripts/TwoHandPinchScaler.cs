using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class TwoHandPinchScaler : MonoBehaviour
{
    private SimpleObjectSpawner spawner;
    private XRHandSubsystem handSubsystem;

    private bool isLeftPinching = false;
    private bool isRightPinching = false;

    private float initialDistance;
    private Vector3 initialScale;

    void Start()
    {
        spawner = GetComponent<SimpleObjectSpawner>();
        handSubsystem = XRGeneralSettings.Instance
            .Manager.activeLoader
            .GetLoadedSubsystem<XRHandSubsystem>();
    }

    void Update()
    {
        if (spawner == null || spawner.spawnedObject == null || handSubsystem == null)
            return;

        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        isLeftPinching = IsPinching(leftHand);
        isRightPinching = IsPinching(rightHand);

        if (isLeftPinching && isRightPinching)
        {
            Vector3 left = GetJointPos(leftHand, XRHandJointID.IndexTip);
            Vector3 right = GetJointPos(rightHand, XRHandJointID.IndexTip);
            float currentDistance = Vector3.Distance(left, right);

            if (initialDistance == 0f)
            {
                initialDistance = currentDistance;
                initialScale = spawner.spawnedObject.transform.localScale;
            }

            float scaleFactor = currentDistance / initialDistance;
            Vector3 newScale = initialScale * scaleFactor;
            newScale = ClampVector(newScale, spawner.minScale, spawner.maxScale);
            spawner.spawnedObject.transform.localScale = newScale;
        }
        else
        {
            initialDistance = 0f;
        }
    }

    bool IsPinching(XRHand hand)
    {
        if (!hand.isTracked) return false;
        Vector3 thumb = GetJointPos(hand, XRHandJointID.ThumbTip);
        Vector3 index = GetJointPos(hand, XRHandJointID.IndexTip);
        return Vector3.Distance(thumb, index) < 0.03f;
    }

    Vector3 GetJointPos(XRHand hand, XRHandJointID jointId)
    {
        XRHandJoint joint = hand.GetJoint(jointId);
        if (joint != null && joint.TryGetPose(out Pose pose))
            return pose.position;
        return Vector3.zero;
    }

    Vector3 ClampVector(Vector3 vec, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(vec.x, min, max),
            Mathf.Clamp(vec.y, min, max),
            Mathf.Clamp(vec.z, min, max)
        );
    }
}
