using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Pollution Settings")]
    [SerializeField]
    TMP_Text Pollution;

    [SerializeField]
    Color clearFog,
        hazyFog;

    [SerializeField]
    float minHaze,
        maxHaze;

    [Range(0f, 100f)]
    [SerializeField]
    float hazeLevel;

    [Header("Traffic Settings")]
    [SerializeField]
    TMP_Text Traffic;

    [SerializeField]
    Transform carContainer;

    [SerializeField]
    [Range(0f, 100f)]
    float trafficLevel;

    [SerializeField]
    int totalNumCars,
        currentNumCars;

    [Header("Cost / Profit Settings")]
    [SerializeField]
    TMP_Text Cost;

    [SerializeField]
    TMP_Text Profit;

    [Range(0.00f, 10000000.00f)]
    [SerializeField]
    float costLevel,
        profitLevel;

    string apiUrl =
        "https://raw.githubusercontent.com/ckwk/cpi-interactive-demo/main/Assets/TestAPI/test-data.tsv";
    public List<string> spreadsheet;
    int currentRowIndex = 1;
    float deltaTimeCount;

    void Start()
    {
        foreach (Transform carRectangle in carContainer)
        {
            foreach (Transform carLine in carRectangle)
            {
                if (carLine.gameObject.name == "MovementPoints")
                    continue;
                totalNumCars += carLine.childCount;
            }
        }
        currentNumCars = totalNumCars;

        // TODO delete this to put in GoogleSheets API calls in Update when access is given
        StartCoroutine(FetchAPI(apiUrl));
    }

    void Update()
    {
        deltaTimeCount += Time.deltaTime;

        // TODO update this once we determine how to interact with GoogleSheets API
        if (deltaTimeCount > 0.1f && currentRowIndex < spreadsheet.Count) // every tenth of a second
        {
            var currentRowArray = spreadsheet[currentRowIndex].Split('\t').ToList();
            UpdateFog(float.Parse(currentRowArray[0]));
            UpdateTraffic(float.Parse(currentRowArray[1]));
            UpdateCost(float.Parse(currentRowArray[2]));
            UpdateProfit(float.Parse(currentRowArray[3]));
            deltaTimeCount = 0;
            currentRowIndex++;
        }
    }

    void UpdateFog(float hazeLevel)
    {
        Pollution.text = "Pollution Level: " + hazeLevel;
        RenderSettings.fogDensity = minHaze + (maxHaze * (hazeLevel / 100f));
        RenderSettings.fogColor = Color.Lerp(clearFog, hazyFog, Math.Clamp(hazeLevel / 50f, 0, 1));
    }

    void UpdateTraffic(float trafficLevel)
    {
        Traffic.text = "Traffic Level: " + trafficLevel;
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

            Transform currentCarLine,
                currentCarRectangle;
            bool firstCarActive,
                lastCarActive;
            do
            {
                currentCarRectangle = carContainer.GetChild(rng.Next(carContainer.childCount));
                currentCarLine = currentCarRectangle.GetChild(
                    rng.Next(1, currentCarRectangle.childCount)
                ); // gets a random carLine transform
                firstCarActive = currentCarLine.GetChild(0).gameObject.activeSelf;
                lastCarActive = currentCarLine
                    .GetChild(currentCarLine.childCount - 1)
                    .gameObject.activeSelf;
            } while (
                (firstCarActive == enableCars && !enableCars)
                || (lastCarActive == enableCars && enableCars)
            );
            var i = enableCars ? currentCarLine.childCount : 1; // determines the direction we enable / disbale cars
            var currentCar = currentCarLine.GetChild(currentCarLine.childCount - i).gameObject; // gets the current car based on above criteria, first if enabling and last if disabling
            while (currentCar.activeSelf == enableCars) // find a car that is not in the desired state
            {
                i -= carDelta;
                currentCar = currentCarLine.GetChild(currentCarLine.childCount - i).gameObject;
            }
            currentCar.SetActive(enableCars); // enable / disable the found car
            currentNumCars += carDelta; // increment the number of cars visible
        }
    }

    void UpdateCost(float costLevel)
    {
        Cost.text = "Costs: $" + costLevel;
    }

    void UpdateProfit(float profitLevel)
    {
        Profit.text = "Profit: $" + profitLevel;
    }

    IEnumerator FetchAPI(string url)
    {
        using (UnityWebRequest apiRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return apiRequest.SendWebRequest();
            if (apiRequest.result != UnityWebRequest.Result.Success)
                print(apiRequest.error);
            else
                spreadsheet = apiRequest.downloadHandler.text
                    .Replace("\r\n", "\n")
                    .Split('\n')
                    .ToList();
        }
    }
}
