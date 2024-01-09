using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Color clearFog,
        hazyFog;

    [SerializeField]
    float minHaze,
        maxHaze;

    [Range(0f, 100f)]
    [SerializeField]
    float hazeLevel;

    float targetHaze;

    [SerializeField]
    Transform carContainer;

    [SerializeField]
    [Range(0f, 100f)]
    float trafficLevel;

    int totalNumCars,
        currentNumCars;

    void Start()
    {
        targetHaze = maxHaze;
        foreach (Transform carLine in carContainer)
        {
            totalNumCars += carLine.childCount;
        }
        currentNumCars = totalNumCars;
    }

    void Update()
    {
        UpdateFog();
        UpdateTraffic();
    }

    void UpdateFog()
    {
        RenderSettings.fogDensity = minHaze + (maxHaze * (hazeLevel / 100f));
        RenderSettings.fogColor = Color.Lerp(clearFog, hazyFog, Math.Clamp(hazeLevel / 50f, 0, 1));
    }

    void UpdateTraffic()
    {
        var targetNumCars = Math.Clamp(
            Math.Ceiling(totalNumCars * (trafficLevel / 100f)),
            10,
            totalNumCars + 1
        );
        while (currentNumCars != targetNumCars)
        {
            System.Random rng = new();
            var carDelta = Math.Sign(targetNumCars - currentNumCars); // returns -1 or 1 to show we are removing or adding cars
            var enableCars = carDelta > 0; // gets whether we want to enable or disable cars
            var currentCarLine = carContainer.GetChild(rng.Next(carContainer.childCount)); // gets a random carLine transform
            var i = enableCars ? currentCarLine.childCount : 1; // determines the direction we enable / disbale cars
            var currentCar = currentCarLine.GetChild(currentCarLine.childCount - i).gameObject; // gets the current car based on above criteria, first if enabling and last if disabling
            while (currentCar.activeSelf == enableCars) // find a car that is not in the desired state
            {
                i += -carDelta;
                currentCar = currentCarLine.GetChild(currentCarLine.childCount - i).gameObject;
            }
            currentCar.SetActive(enableCars); // enable / disable the found car
            currentNumCars += carDelta; // increment the number of cars visible
        }
    }
}
