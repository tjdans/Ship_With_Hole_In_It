using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class MinimapCamera : MonoBehaviour
{
    [Header("Follw Settings")]
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.position.x, target.position.y + 33.0f, target.position.z);
    }
}
