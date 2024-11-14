using UnityEngine;

public class PerkSystem : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void LevelUpAttack()
    {
        if (playerController.AttackDamage < 5)
        {
            playerController.AttackDamage += 1;
            Debug.Log("Attack Damage increased to: " + playerController.AttackDamage);
            playerController.ClosePerkMenu(); // Close the perk menu after selecting a perk
        }
    }

    public void LevelUpSpeed()
    {
        if (playerController.CurrentSpeed < playerController.BaseSpeed * 1.25f)
        {
            playerController.CurrentSpeed += playerController.BaseSpeed * 0.05f;
            Debug.Log("Speed increased to: " + playerController.CurrentSpeed);
            playerController.ClosePerkMenu(); // Close the perk menu after selecting a perk
        }
    }

    public void LevelUpHealth()
    {
        // Increase MaxHealth and CurrentHealth accordingly
        if (playerController.MaxHealth < 25) // You can adjust this cap as necessary
        {
            playerController.MaxHealth += 3;
            playerController.CurrentHealth = Mathf.Min(playerController.CurrentHealth + 3, playerController.MaxHealth); // Ensure current health doesn't exceed max health

            Debug.Log("Max Health increased to: " + playerController.MaxHealth);
            playerController.UpdateHealthDisplay(); // Update the health UI display to show new MaxHealth and CurrentHealth

            playerController.ClosePerkMenu(); // Close the perk menu after selecting a perk
        }
    }
}
