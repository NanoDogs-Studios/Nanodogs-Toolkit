// © 2025 Nanodogs Studios. All rights reserved.

using UnityEngine;

namespace Nanodogs.API.Explosion
{
    public class ExplosionAPI : MonoBehaviour
    {
        public static bool useCameraShake = true;

        // Method to create an explosion at a given position with specified settings
        /// <summary>
        /// creates an explosion at the specified position using the provided settings.
        /// </summary>
        /// <param name="position">where the explosion will be created in world space</param>
        /// <param name="settings">the provided settings, see ExplosionSettings for more info.</param>
        /// <returns></returns>
        public static GameObject Explosion(Vector3 position, ExplosionSettings settings)
        {
            // Instantiate explosion effect
            if (settings.explosionEffectPrefab != null)
            {
                GameObject explosionEffect = Instantiate(settings.explosionEffectPrefab, position, Quaternion.identity);
                Destroy(explosionEffect, 2f); // Destroy effect after 5 seconds
            }
            if (useCameraShake)
            {
                // Trigger camera shake effect
                
            }
            // Find all colliders in the explosion radius
            Collider[] colliders = Physics.OverlapSphere(position, settings.radius);
            foreach (Collider hit in colliders)
            {
                // Apply explosion force to rigidbodies
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(settings.force, position, settings.radius, settings.upwardsModifier, settings.forceMode);
                    rb.useGravity = settings.useGravity;
                }

                // Apply damage to objects with a Health component
                // add this!!!!
            }
            return null; // Return null or any relevant information if needed
        }

    }
}