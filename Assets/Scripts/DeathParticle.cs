using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    [SerializeField] private GameObject _deathParticle;

    private void OnDestroy()
    {
        Instantiate(_deathParticle, transform.position, transform.rotation);
    }
}
