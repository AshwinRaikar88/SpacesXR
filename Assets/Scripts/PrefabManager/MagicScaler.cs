using UnityEngine;
using Oculus.Interaction.Input;

public class MagicScaler : MonoBehaviour
{
    [SerializeField]
    private Hand leftHand;

    [SerializeField]
    private Hand rightHand;

    [SerializeField]
    public GameObject targetObject;

    [SerializeField]
    private float scaleSpeed = 5f;

    private bool wasPinching = false;
    private float initialDistance = 0f;
    private Vector3 initialScale;

    private Transform modelTransform;

    void Start()
    {
        if (targetObject != null)
        {
            // Try to find the "model" GameObject inside "Visuals"
            modelTransform = targetObject.transform.Find("Visuals/model");

            if (modelTransform == null)
            {
                Debug.LogWarning("MagicScaler: Could not find 'Visuals/model' in targetObject. Falling back to root.");
                modelTransform = targetObject.transform;
            }
        }
    }

    public void SetTargetObject(GameObject newTarget)
    {
        targetObject = newTarget;

        if (targetObject != null)
        {
            modelTransform = targetObject.transform.Find("Visuals/model");

            if (modelTransform == null)
            {
                Debug.LogWarning("MagicScaler: Could not find 'Visuals/model'. Using root instead.");
                modelTransform = targetObject.transform;
            }
        }
        else
        {
            modelTransform = null;
        }
    }


    void Update()
    {
        if (leftHand == null || rightHand == null || modelTransform == null)
            return;

        bool leftPinch = leftHand.GetFingerIsPinching(HandFinger.Index);
        bool rightPinch = rightHand.GetFingerIsPinching(HandFinger.Index);

        if (leftPinch && rightPinch)
        {
            // Get fingertip poses
            if (leftHand.GetJointPose(HandJointId.HandIndexTip, out Pose leftPose) &&
                rightHand.GetJointPose(HandJointId.HandIndexTip, out Pose rightPose))
            {
                float currentDistance = Vector3.Distance(leftPose.position, rightPose.position);

                if (!wasPinching)
                {
                    initialDistance = currentDistance;
                    initialScale = modelTransform.localScale;
                    wasPinching = true;
                }
                else
                {
                    float scaleFactor = currentDistance / initialDistance;
                    Vector3 newScale = initialScale * scaleFactor;

                    // Optional: Clamp scale
                    newScale = Vector3.Max(Vector3.one * 0.1f, Vector3.Min(newScale, Vector3.one * 10f));

                    modelTransform.localScale = Vector3.Lerp(modelTransform.localScale, newScale, Time.deltaTime * scaleSpeed);
                }
            }
        }
        else
        {
            wasPinching = false;
        }
    }
}
