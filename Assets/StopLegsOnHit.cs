using UnityEngine;
using UnityEngine.AI;

public class StopLegsOnHit : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _anim;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_agent == null || _anim == null) return;

        // Если скелет физически перемещается — анимация идет с обычной скоростью (1)
        // Как только скорость падает (остановился) — кадр замирает (0)
        _anim.speed = (_agent.velocity.magnitude > 0.15f) ? 1f : 0f;
    }
}