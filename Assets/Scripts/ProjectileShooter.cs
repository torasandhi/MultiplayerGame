using PrimeTween;
using PurrNet;
using UnityEngine;

public class ProjectileShooter : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask hitMask;


    [Header("Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float speed = 50f;

    private Player _player;

    protected override void OnSpawned()
    {
        base.OnSpawned();
        _player = GetComponent<Player>();
    }

    public void ShootProjectile(string tag, Vector3 startPosition, Vector3 forwardDirection, Quaternion rotation, Transform parent = null)
    {
        var projectile = ObjectPoolManager.Instance.SpawnFromPool(tag, startPosition, rotation);
        if (projectile == null) { Debug.LogWarning($"Failed to spawn projectile with tag: {tag}"); return; }

        if (projectile.TryGetComponent<Projectile>(out var proj))
        {
            proj.SetDamage(damage);
            Debug.Log($"Damage is set to : {damage}");
        }

        Camera cam = _player.PlayerCamera;
        Vector3 rayOrigin = cam.transform.position;
        Vector3 rayDirection = cam.transform.forward;
        Vector3 targetPoint;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, hitMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = rayOrigin + rayDirection * maxDistance;
        }

        projectile.position = startPosition;
        projectile.rotation = rotation;

        float distance = Vector3.Distance(startPosition, targetPoint);
        float duration = distance / speed;

        Tween.Position(
            projectile,
            targetPoint,
            duration,
            Ease.Linear
        )
        .OnComplete(() =>
        {
            ObjectPoolManager.Instance.Despawn(projectile);
        });
    }


    public void Shoot()
    {
        Camera cam = _player.PlayerCamera;
        Vector3 rayOrigin = cam.transform.position;
        Vector3 rayDirection = cam.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxDistance, hitMask))
        {
            if (!hit.transform.TryGetComponent<IDamageable>(out var damageable))
                return;
            damageable.IChangeHealth(-(int)damage);

            if (!hit.transform.TryGetComponent<Player>(out var player))
                return;
            Debug.Log($"Enemy Health : {player.Health}");
        }
    }

    [ServerRpc(requireOwnership: false)]
    public void ShootProjectileServer(string tag, Vector3 startPosition, Vector3 forwardDirection, Quaternion rotation)
    {
        ShootProjectileClient(tag, startPosition, forwardDirection, rotation);
    }

    [ObserversRpc(excludeOwner: true)]
    public void ShootProjectileClient(string tag, Vector3 startPosition, Vector3 forwardDirection, Quaternion rotation)
    {
        ShootProjectile(tag, startPosition, forwardDirection, rotation, ObjectPoolManager.Instance.transform);
    }
}
