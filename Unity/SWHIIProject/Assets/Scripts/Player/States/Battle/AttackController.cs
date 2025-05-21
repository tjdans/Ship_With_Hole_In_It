using UnityEngine;

public class AttackController : MonoBehaviour
{
    public TestEnemy CurrentTarget;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _targetRange = 10f;
    [SerializeField] private float _stopRange = 2f;
    [SerializeField] private Collider[] _hitColliders = new Collider[10];

    public void SetTarget()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 15f, _hitColliders, _enemyLayer);
        TestEnemy closestEnemy = null;

        for (int i = 0; i < numColliders; i++)
        {
            Collider enemyColl = _hitColliders[i];
            if (enemyColl != null)
            {
                float distance = Vector3.Distance(transform.position,
                                        enemyColl.ClosestPoint(transform.position));
                if (distance < _targetRange)
                {
                    closestEnemy = enemyColl.GetComponent<TestEnemy>();
                }
            }
        }

        if (closestEnemy != null)
        {
            float minDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

            if (minDistance > _stopRange)
            {
                CurrentTarget = closestEnemy;
            }
        }
        else
        {
            CurrentTarget = null;
        }
    }
}