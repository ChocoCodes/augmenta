using System.Collections;
using UnityEngine;

public class RobotFighter : MonoBehaviour
{
    [SerializeField] private float jabDistance = 0.12f;
    [SerializeField] private float jabLeanAngle = 20f;
    [SerializeField] private float jabSpeed = 12f;

    private bool isJabbing = false;
    private Vector3 originalPositionWorld;
    private Quaternion originalRotationWorld;

    void Start()
    {
        originalPositionWorld = transform.position;
        originalRotationWorld = transform.rotation;
    }

    public void PerformJab()
    {
        if (!isJabbing)
        {
            Debug.Log("RobotFighter: PerformJab called.");
            StartCoroutine(JabRoutine());
        }
    }

    private IEnumerator JabRoutine()
    {
        isJabbing = true;

        originalPositionWorld = transform.position;
        originalRotationWorld = transform.rotation;

        Vector3 targetPos = originalPositionWorld + transform.forward * jabDistance;
        Quaternion targetRot = originalRotationWorld * Quaternion.Euler(jabLeanAngle, 0f, 0f);

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * jabSpeed;
            transform.position = Vector3.Lerp(originalPositionWorld, targetPos, t);
            transform.rotation = Quaternion.Lerp(originalRotationWorld, targetRot, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * jabSpeed;
            transform.position = Vector3.Lerp(targetPos, originalPositionWorld, t);
            transform.rotation = Quaternion.Lerp(targetRot, originalRotationWorld, t);
            yield return null; 
        }

        transform.position = originalPositionWorld;
        transform.rotation = originalRotationWorld;
        isJabbing = false;
    }
}