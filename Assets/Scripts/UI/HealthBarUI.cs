using RPG.State;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject HealBarPrefab;

    public Transform HealthBarPoint;

    public bool AlwaysVisible;

    public float VisibleTime = 3f;

    private float visibleTimeLeft;
    private Image healthBar;
    private Transform healthBarHolder;
    private Transform cameraTrans;
    private CharacterStates currentStates;

    private void Awake()
    {
        currentStates = GetComponent<CharacterStates>();
        currentStates.OnUpdateHealthBarUI += UpdateHealthBarUI;
        visibleTimeLeft = VisibleTime;
    }

    private void OnEnable()
    {
        cameraTrans = Camera.main.transform;

        foreach (var canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace && canvas.CompareTag("Health Canvas"))
            {
                healthBarHolder = Instantiate(HealBarPrefab, canvas.transform).transform;
                healthBar = healthBarHolder.GetChild(0).GetComponent<Image>();
                healthBarHolder.gameObject.SetActive(AlwaysVisible);
            }
        }
    }

    private void LateUpdate()
    {
        if (healthBarHolder != null && HealthBarPoint != null) 
        {
            healthBarHolder.position = HealthBarPoint.position;
            healthBarHolder.forward = -cameraTrans.forward;

            if (visibleTimeLeft <= 0 && !AlwaysVisible)
                healthBarHolder.gameObject.SetActive(false);
            else
            { 
                visibleTimeLeft -= Time.deltaTime;
                healthBar.fillAmount = currentStates.CurrentHealth / currentStates.MaxHealth;
            }
        }
    }

    private void UpdateHealthBarUI(float currentHealth, float maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(healthBarHolder.gameObject);
        healthBarHolder.gameObject.SetActive(true);
        healthBar.fillAmount = currentHealth / maxHealth;

        visibleTimeLeft = VisibleTime;
    }
}
