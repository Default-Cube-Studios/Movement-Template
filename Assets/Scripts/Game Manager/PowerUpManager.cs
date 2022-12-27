using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[DisallowMultipleComponent]
public class PowerUpManager : MonoBehaviour
{
    public PowerUp PowerUp;
    [Header("Timer")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("How long the power up lasts for")] public float duration;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(Players.ActivePlayer.gameObject.tag))
            return;

        PowerUp.OnCollect.Invoke();

        if (isTimed)
            StartCoroutine(RevertPowerUp());
        else
            Destroy(gameObject);
    }

    IEnumerator RevertPowerUp()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(duration);
        PowerUp.OnRevert.Invoke();
        Destroy(gameObject);
    }
}