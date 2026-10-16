using UnityEngine;

public class TestTargetLock : MonoBehaviour
{
    public IsometricCamera cameraScript;

    void Update()
    {
        // Press Tab to toggle lock-on.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (cameraScript.isLocked)
            {
                cameraScript.Unlock();
            }
            else
            {
                cameraScript.LockOn();
            }
        }
    }
}
