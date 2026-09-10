using UnityEngine;
using UnityEngine.InputSystem;

namespace Ludocore
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Animator animator;
        [SerializeField] private GrabAnchor anchor;
        [SerializeField] private InputActionReference attackAction;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (attackAction != null && attackAction.action != null)
                attackAction.action.Enable();
        }

        private void Update()
        {
            bool isAttacking = false;

            // 1. Проверка через Input Action
            if (attackAction != null && attackAction.action != null && attackAction.action.WasPressedThisFrame())
                isAttacking = true;

            // 2. Гарантированный перехват клика мыши напрямую
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                isAttacking = true;

            if (!isAttacking) return;

            Debug.Log("<color=green>Клик зарегистрирован!</color>");

            // Запуск анимации
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                Debug.Log("<color=cyan>Триггер Attack отправлен в Animator!</color>");
            }
            else
            {
                Debug.LogError("Animator не найден на объекте!");
            }

            // Выстрел оружия (если используется Weapon)
            if (anchor != null && anchor.Held != null)
            {
                if (anchor.Held.TryGetComponent(out Weapon weapon))
                    weapon.Fire();
            }
        }
    }
}