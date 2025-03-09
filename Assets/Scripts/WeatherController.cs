using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Created By: Alex Barnett
/// Controls weather effects in the game, including fog, rain, and lens flare effects.
/// Allows dynamic changes to weather conditions through a user interface.
/// </summary>
public class WeatherController : MonoBehaviour
{
    [SerializeField] private Material fogMat;
    [SerializeField] private Material lensFlareMat;
    [SerializeField] private Light sun;
    [SerializeField] private VisualEffect rainEffect;
    [SerializeField] private float changeTime;

    /// <summary>
    /// Represents a set of parameters defining a specific weather condition.
    /// </summary>
    private class WeatherCondition
    {
        public float FogDensity { get; private set; }
        public bool ShowSkybox { get; private set; }
        public int SunTemp { get; private set; }
        public bool Raining { get; private set; }
        public float FlareMinBrightness { get; private set; }
        public float FlareSpillover { get; private set; }

        public WeatherCondition(float fogDensity, bool showSkybox, int sunTemp, bool raining, float flareMinBrightness, float flareSpillover)
        {
            FogDensity = fogDensity;
            ShowSkybox = showSkybox;
            SunTemp = sunTemp;
            Raining = raining;
            FlareMinBrightness = flareMinBrightness;
            FlareSpillover = flareSpillover;
        }
    }

    // Preset weather conditions
    private WeatherCondition clearSkies = new WeatherCondition(1f, true, 5000, false, 3, 14f);
    private WeatherCondition fog = new WeatherCondition(1.5f, false, 8000, false, 5, 17f);
    private WeatherCondition rain = new WeatherCondition(2f, true, 8000, true, 25f, 16f);
    private WeatherCondition fogRain = new WeatherCondition(2f, false, 9000, true, 3f, 14f);

    private void Start()
    {
        // Set initial weather condition
        ChangeWeatherButton(1);
    }

    /// <summary>
    /// Changes weather conditions over time to create a smooth transition.
    /// </summary>
    private IEnumerator ChangeWeatherOverTime(WeatherCondition selectedWeather)
    {
        // Capture starting conditions for interpolation
        float startFogHeight = fogMat.GetFloat("_FogMinHeight");
        float endFogHeight = selectedWeather.ShowSkybox ? 1450 : 8000f;
        float startSunTemp = sun.colorTemperature;
        float endSunTemp = selectedWeather.SunTemp;
        float startFogDensity = fogMat.GetFloat("_FogDensity");
        float endFogDensity = selectedWeather.FogDensity;
        int startRainRate = rainEffect.GetInt("Spawn Rate");
        int endRainRate = selectedWeather.Raining ? 300000 : 0;
        float startFlareMinBrightness = lensFlareMat.GetFloat("_MinBrightness");
        float startFlareSpillover = lensFlareMat.GetFloat("_SpilloverIntensity");
        float endFlareMinBrightness = selectedWeather.FlareMinBrightness;
        float endFlareSpillover = selectedWeather.FlareSpillover;

        float elapsedTime = 0;

        while (elapsedTime < changeTime)
        {
            float t = elapsedTime / changeTime;
            ApplyWeatherChanges(t, startFogHeight, endFogHeight, startSunTemp, endSunTemp, startFogDensity, endFogDensity, startRainRate, endRainRate, startFlareMinBrightness, endFlareMinBrightness, startFlareSpillover, endFlareSpillover);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Apply final weather conditions
        ApplyWeatherChanges(1f, startFogHeight, endFogHeight, startSunTemp, endSunTemp, startFogDensity, endFogDensity, startRainRate, endRainRate, startFlareMinBrightness, endFlareMinBrightness, startFlareSpillover, endFlareSpillover);
    }

    /// <summary>
    /// Applies weather condition changes based on the given interpolation factor.
    /// </summary>
    private void ApplyWeatherChanges(float t, float startFogHeight, float endFogHeight, float startSunTemp, float endSunTemp, float startFogDensity, float endFogDensity, int startRainRate, int endRainRate, float startFlareMinBrightness, float endFlareMinBrightness, float startFlareSpillover, float endFlareSpillover)
    {
        fogMat.SetFloat("_FogMinHeight", Mathf.Lerp(startFogHeight, endFogHeight, t));
        fogMat.SetFloat("_FogDensity", Mathf.Lerp(startFogDensity, endFogDensity, t));
        rainEffect.SetInt("Spawn Rate", Mathf.RoundToInt(Mathf.Lerp(startRainRate, endRainRate, t)));
        sun.colorTemperature = Mathf.Lerp(startSunTemp, endSunTemp, t);
        lensFlareMat.SetFloat("_MinBrightness", Mathf.Lerp(startFlareMinBrightness, endFlareMinBrightness, t));
        lensFlareMat.SetFloat("_SpilloverIntensity", Mathf.Lerp(startFlareSpillover, endFlareSpillover, t));
    }

    /// <summary>
    /// Public method to change weather conditions based on a button input.
    /// </summary>
    public void ChangeWeatherButton(int weather)
    {
        WeatherCondition selected = weather switch
        {
            1 => clearSkies,
            2 => fog,
            3 => rain,
            4 => fogRain,
            _ => new WeatherCondition(0, false, 0, false, 100, 0)
        };
        StopAllCoroutines();
        StartCoroutine(ChangeWeatherOverTime(selected));
    }
}

