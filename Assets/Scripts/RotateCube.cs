using System.Collections;
using UnityEngine;
// Test script to check out rotation on a cube
public class RotateCube : MonoBehaviour {
    // Define possible cube rotation states
    Quaternion[] states = {
        Quaternion.identity * Quaternion.Euler(0, 0, 0),
        Quaternion.identity * Quaternion.Euler(-90, 0, 0),
        Quaternion.identity * Quaternion.Euler(-180, 0, 0),
        Quaternion.identity * Quaternion.Euler(-270, 0, 0)
    };
    // counter to keep track of current rotation state
    int i = 0;
    // Coroutine to rotate the digits
    public IEnumerator Rotate() {
        i++;
        if (i > 0 && i%4==0) {
            i = 0;
        }
        while (transform.rotation != states[i]) {
            transform.rotation = Quaternion.Lerp(transform.rotation, states[i], 0.1f);
            yield return new WaitForSeconds(0);
        }
    }
}
