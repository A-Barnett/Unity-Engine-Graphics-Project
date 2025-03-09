using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Created by: Alex Barnett
/// Controls the transition of time by rotating a directional light (sun) and adjusting post-processing effects.
/// </summary>
public class TimeController : MonoBehaviour
{
    [SerializeField] private float timeChangeTime; // Duration for time change transition
    [SerializeField] private Light sun; // Directional light representing the sun
    [SerializeField] private Volume postProcessing; // Post-processing volume to control visual effects

    // Post-processing effect parameters
    private LiftGammaGain liftGammaGain;
    private SplitToning splitToning;

    // Day/Night effect values
    private float daySplitBalance = 100;
    private float nightSplitBalance = 30;
    private float dayGain = -0.1f;
    private float nightGain = 0f;
    private float dayLift = -0.05f;
    private float nightLift = 0.1f;

    private bool isCurrentlyNight; // Tracks if it is currently night time

    private void Start()
    {
        // Attempt to retrieve the post-processing effects from the volume's profile
        postProcessing.profile.TryGet(out liftGammaGain);
        postProcessing.profile.TryGet(out splitToning);
    }

    /// <summary>
    /// Starts the time change process based on the target angle for the sun.
    /// </summary>
    /// <param name="angle">The target angle for the sun's rotation.</param>
    public void SetTimeButtons(float angle)
    {
        StopAllCoroutines(); // Stop any existing time change coroutines
        bool isNight = angle == 270; // Determine if the target angle corresponds to night time
        StartCoroutine(ChangeTime(angle, isNight));
    }

    /// <summary>
    /// Smoothly transitions the sun's rotation and post-processing effects to represent a change in time of day.
    /// </summary>
    /// <param name="targetAngle">Target angle for the sun's rotation.</param>
    /// <param name="isNight">Flag indicating whether it's transitioning to night time.</param>
    private IEnumerator ChangeTime(float targetAngle, bool isNight)
    {
        float elapsed = 0;
        Quaternion startRotation = sun.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetAngle, sun.transform.rotation.eulerAngles.y, sun.transform.rotation.eulerAngles.z);
        bool changeTimeOfDay = isNight != isCurrentlyNight;

        while (elapsed < timeChangeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timeChangeTime;

            // Interpolate the sun's rotation
            sun.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            // If there is a change in time of day, interpolate post-processing effects
            if (changeTimeOfDay)
            {
                UpdatePostProcessingEffects(isNight, t);
            }

            yield return null;
        }

        sun.transform.rotation = endRotation;
        // Ensure the final state is set after the transition
        if (changeTimeOfDay)
        {
            FinalizeTimeChange(isNight);
        }
    }

    /// <summary>
    /// Updates post-processing effect parameters based on the time of day.
    /// </summary>
    private void UpdatePostProcessingEffects(bool isNight, float t)
    {
        float currentSplitBalance = Mathf.Lerp(isNight ? daySplitBalance : nightSplitBalance, isNight ? nightSplitBalance : daySplitBalance, t);
        float currentGain = Mathf.Lerp(isNight ? dayGain : nightGain, isNight ? nightGain : dayGain, t);
        float currentLift = Mathf.Lerp(isNight ? dayLift : nightLift, isNight ? nightLift : dayLift, t);

        liftGammaGain.gain.value = new Vector4(0, 0, 0, currentGain);
        liftGammaGain.lift.value = new Vector4(0, 0, 0, currentLift);
        splitToning.balance.Override(currentSplitBalance);
    }

    /// <summary>
    /// Finalizes the time change by setting the post-processing effects to their target values.
    /// </summary>
    private void FinalizeTimeChange(bool isNight)
    {
        liftGammaGain.gain.value = new Vector4(0, 0, 0, isNight ? nightGain : dayGain); // Final gain value
        liftGammaGain.lift.value = new Vector4(0, 0, 0, isNight ? nightLift : dayLift); // Final lift value
        splitToning.balance.Override(isNight ? nightSplitBalance : daySplitBalance); // Final split tone balance
        isCurrentlyNight = isNight; // Update the current time of day
    }
}
