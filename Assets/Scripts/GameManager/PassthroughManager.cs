using UnityEngine;
// using Oculus.XR.Passthrough;  // From Meta XR SDK

public class PassthroughManager : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public GameObject virtualEnvironment;

    void Start()
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = true;
        }
    }

    public void Toggle()
    {
        if (passthroughLayer != null)
        {
            bool isActive = passthroughLayer.enabled;
            virtualEnvironment.SetActive(isActive);
            passthroughLayer.enabled = !isActive;
            
        }
    }

    public void SetPassthrough(bool isActive)
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = isActive;            
            virtualEnvironment.SetActive(!isActive);            
        }
    }
}
