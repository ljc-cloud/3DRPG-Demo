using RPG.Character.Wepon;
using RPG.Tools;
using System;
using UnityEngine;

namespace RPG.Manager
{
    public class MouseManager : Singleton<MouseManager>
    {
        private RaycastHit hitInfo;
        public event Action<Vector3> OnMouseClicked;
        public event Action<GameObject> OnEnemyClicked;
        public Texture2D Point, Doorway, Attack, Target, Arrow;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            ChangeMouseTexture();
            MouseControl();
        }

        void ChangeMouseTexture()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                // «–ªª Û±ÍÃ˘Õº
                switch (hitInfo.collider.gameObject.tag)
                {
                    case "Ground":
                        Cursor.SetCursor(Target, new Vector2(16, 16), CursorMode.Auto);
                        break;
                    case "Enemy":
                        Cursor.SetCursor(Attack, new Vector2(16, 16), CursorMode.Auto);
                        break;
                    case "Attackable":
                        if (!IsRockFlyToPlayer(hitInfo))
                        {
                            Cursor.SetCursor(Attack, new Vector2(16, 16), CursorMode.Auto);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void MouseControl()
        {
            if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
            {
                if (hitInfo.collider.gameObject.CompareTag("Ground"))
                {
                    OnMouseClicked?.Invoke(hitInfo.point);
                    return;
                }
                if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
                    return;
                }
                if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                {
                    if (!IsRockFlyToPlayer(hitInfo))
                    {
                        OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
                    }
                }
            }
        }

        private bool IsRockFlyToPlayer(RaycastHit hitInfo)
        {
            if (hitInfo.collider.gameObject.GetComponent<Rock>())
            {
                return hitInfo.collider.gameObject.GetComponent<Rock>().RockStates == RockStates.HIT_PLAYER;
            }
            return false;
        }
    }
}

