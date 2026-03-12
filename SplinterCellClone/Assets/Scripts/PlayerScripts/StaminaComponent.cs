using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    [SerializeField] float staminaPool;
    [SerializeField] float staminaLossRate;
    [SerializeField] float staminaRechargeRate;
    [SerializeField] float rechargeDelayTimer;

    float currentStamina;
    float rechargeTimer;

    bool staminaEmpty;
    bool usingStamina;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStamina = staminaPool;
        rechargeTimer = rechargeDelayTimer;
        UpdateStaminaMeter();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStaminaMeter();

        if (usingStamina)
        {
            DepleteStamina();
            rechargeTimer = rechargeDelayTimer;
        }
        else
        {
            if (currentStamina < staminaPool)
            {
                rechargeTimer -= Time.deltaTime;

                if (rechargeTimer <= 0)
                {
                    RechargeStamina();
                }
            }
        }

    }



    void DepleteStamina()
    {
        currentStamina -= staminaLossRate * Time.deltaTime;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            staminaEmpty = true;
        }
    }

    void RechargeStamina()
    {
        currentStamina += staminaRechargeRate * Time.deltaTime;

        if (currentStamina >= staminaPool)
        {
            currentStamina = staminaPool;
        }
    }

    void UpdateStaminaMeter()
    {
        GameManager.instance.playerStamBar.fillAmount = currentStamina / staminaPool;

        Debug.Log("Calling Stam meter funciton!");
    }


    public void SetUsingStamina(bool state)
    {
        usingStamina = state;
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }
}
