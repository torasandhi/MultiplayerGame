using PrimeTween;
using System.Data;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Tween _tween;

    private float damage = 10f;
    public float Damage => damage;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        // stop tween if any
        if (_tween.isAlive)
        {
            try { _tween.Stop(); } catch { }
        }
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var hitPlayer))
        {
            hitPlayer.IChangeHealth(-Mathf.CeilToInt(damage));
        }

        ObjectPoolManager.Instance.Despawn(transform);
    }
}