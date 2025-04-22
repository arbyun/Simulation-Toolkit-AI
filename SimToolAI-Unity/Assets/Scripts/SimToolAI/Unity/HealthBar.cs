using UnityEngine;
using UnityEngine.UI;

namespace SimToolAI.Unity
{
    /// <summary>
    /// Health bar component for visualizing entity health
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Gradient gradient;
        [SerializeField] private Image fill;
        
        /// <summary>
        /// Sets the maximum health
        /// </summary>
        /// <param name="health">Maximum health</param>
        public void SetMaxHealth(int health)
        {
            slider.maxValue = health;
            slider.value = health;
            
            fill.color = gradient.Evaluate(1f);
        }
        
        /// <summary>
        /// Sets the current health
        /// </summary>
        /// <param name="health">Current health</param>
        public void SetHealth(int health)
        {
            slider.value = health;
            
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}