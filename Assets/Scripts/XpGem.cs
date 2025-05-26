using UnityEngine;

public class XpGem : MonoBehaviour
{
    public int xp;
    public float attractionSpeed;

    private bool _isAttracting;
    private Transform _target;

    public void AttractToPlayer(Transform target)
    {
        if (_isAttracting)
        {
            return;
        }
        
        _target = target;
        _isAttracting = true;
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (_target is null)
        {
            return;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, _target.position, attractionSpeed * Time.deltaTime);
    }
}