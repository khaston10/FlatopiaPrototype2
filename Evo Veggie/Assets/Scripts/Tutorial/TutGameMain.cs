﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutGameMain : MonoBehaviour
{
    public int worldSize;
    public int foodSpawned;
    public int plantEaters;
    public int meatEaters;
    public int gamePoints;
    public int daysBetweenMeatEaterSpawn;
    public int daysUntilMeatEaterStarves;
    public int meaterEaterSpawnCounter = 0;
    public bool creaturesAwake = true;
    public bool gamePaused = false;
    public bool slayerUpgradeUnlocked = false;
    public bool freezeUpgradeUnlocked = false;
    public bool feedUpgradeUnlocked = false;
    public bool caffeineUpgradeUnlocked = false;
    public bool caffeineSpeedOn = false;
    public bool meatEatersFrozen = false;
    public bool meatEatersSpawnDisabled = false; // This is used to temporaly disable meat eater spawn when user kills meat eaters.
    public float plantEaterSpeed = .5f; // Set to .5 in normal speed and 2 for Fast Forward mode.
    public float caffeinePlantEaterSpeed = 1.5f; // Set to 1.5 in normal speed and 6 for Fast Forward mode.
    public float meatEaterSpeed = .5f; // Set to .5 in normal speed and 2 for Fast Forward mode.
    public float slowSpeedMeter = .001f; // Set to .001 in normal speed and .004 for Fast Forward mode.
    public float medSpeedMeter = .01f; // Set to .01 in normal speed and .04 for Fast Forward mode.
    public Light star1;
    public Text GameSpeedDisplayText;
    public Text gamePointsText;
    public Text worldSizeText;
    public Text foodSpawnedText;
    public Text PlantEatersText;
    public Text dayCountText;
    public Transform planet1;
    public Transform star1Body;
    public Transform food;
    public Transform plantEater;
    public Transform meatEater;
    public List<Vector3> foodPositions;
    public List<Vector3> plantEaterStartPositions;
    public List<Transform> foodList;
    public List<Transform> plantEaterList;
    public List<Transform> meatEaterList;
    public int noPlantEatersKilledForXDays = 0;
    private int worldSizeLimit = 30;

    public RawImage landOwnerImage;
    public Texture landOwnerTexture;
    public RawImage ninjaImage;
    public Texture ninjaTexture;
    public RawImage survivalistImage;
    public Texture survivalistTexture;
    public RawImage glutonImage;
    public Texture glutonTexture;
    public RawImage unlockedImage;
    public Texture unlockedTexture;
    public Slider slayerSlider;
    public Slider freezeSlider;
    public Slider feedSlider;
    public Slider caffeineSlider;
    public Text slayerSliderText;
    public Text freezeSliderText;
    public Text feedSliderText;
    public Text caffeineSliderText;
    public Text daysUntilMeatEatersText;
    private int daysUntilMeatEaterCounter;




    //------------------------------------This code is used for debugging, remove before release.--------------------
    public bool foodOn = true;
    public bool movePlanetOn = true;
    public bool moveStarOn = true;
    public bool plantEatersOn = true;
    public bool meathEatersOn = true;

    private int day = 1;
    private float timer = 0.0f;
    private float plantEaterTimer = 0.0f;
    private float lengthOfDay = 25.12f;
    private float timerSpeedCoefficient = .25f; // This value depends on the lengthOfDay. If Day is 360 the coefficient should be 1.
    // If lengthOfDay is 2 *360, or 720 the coefficient should be 1/2 or .5, and so on. 
    private int gameSpeed = 1;
    private float yPos;
    private float xPos;
    private float star1YPos;
    private float star1XPos;
    private float planetYPos;
    private float planetZPos;
    private float planetXPos;
    private Color starvingPlantEater = new Color(.8f, .7f, .3f, .1f);
    private Color starvingMeatEater = new Color(.8f, .7f, .3f, .1f);
    private Color FedPlantEater = new Color(.1f, 1f, .1f, 1f);


    // Start is called before the first frame update
    void Start()
    {
        worldSize = GlobalControl.Instance.worldSize;
        foodSpawned = GlobalControl.Instance.foodSpawned;
        plantEaters = GlobalControl.Instance.plantEaters;
        gamePoints = GlobalControl.Instance.gamePoints;

        // Set sliders as invisible
        slayerSlider.gameObject.SetActive(false);
        freezeSlider.gameObject.SetActive(false);
        feedSlider.gameObject.SetActive(false);
        caffeineSlider.gameObject.SetActive(false);

        daysUntilMeatEaterCounter = daysBetweenMeatEaterSpawn;

        // Populate the positions list with all possible positions for grids.
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                Vector3 foodPos = new Vector3(i, 1f, j);
                Vector3 plantEaterPos = new Vector3(i, 1.4f, j);
                foodPositions.Add(foodPos);
                plantEaterStartPositions.Add(plantEaterPos);
            }
        }

        // Stock food at the beginning of day 1.
        for (int i = 0; i < foodSpawned; i++)
        {
            // Pick a random position.
            int randPos = Random.Range(0, foodPositions.Count);

            // Place food at position and remove its position from the available list, then place the transform in the foodList. 
            Transform f = Instantiate(food);
            f.localPosition = foodPositions[randPos];
            foodPositions.RemoveAt(randPos);
            foodList.Add(f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Start timer for day, when the timer reaches length_of_day the timer reasets, if game is paused then do not update time.
        if (gamePaused != true)
        {
            timer += Time.deltaTime;
            plantEaterTimer += Time.deltaTime;

        }


        //------------------------------------This code is used for debugging, remove before release.--------------------
        if (foodOn == false)
        {
            for (int i = 0; i < foodList.Count; i++)
            {
                foodPositions.Add(foodList[i].localPosition);
                Destroy(foodList[i].gameObject);
            }
            foodList.Clear();
        }

        // Restock plant eaters at the beginning of each day.
        if (plantEaterList.Count < plantEaters && plantEatersOn && creaturesAwake)
        {
            // Pick a random position.
            int randPos = Random.Range(0, plantEaterStartPositions.Count);

            // Place plant eater at position, then place the transform in the plantEaterList. 
            Transform p = Instantiate(plantEater);
            p.localPosition = plantEaterStartPositions[randPos];
            plantEaterList.Add(p);

        }

        // Restock meat eaters at the beginning of the day if is time, then reset the counter.
        if (meaterEaterSpawnCounter == daysBetweenMeatEaterSpawn && meatEaterList.Count < meatEaters && meatEatersSpawnDisabled == false)
        {
            // Pick a random position.
            int randPos = Random.Range(0, plantEaterStartPositions.Count);

            // Place meat eater at position, then place the transform in the meatEaterList. 
            Transform m = Instantiate(meatEater);
            m.localPosition = plantEaterStartPositions[randPos];
            meatEaterList.Add(m);
        }

        else if (meaterEaterSpawnCounter > daysBetweenMeatEaterSpawn) meaterEaterSpawnCounter = 0;

        //------------------------------------This code is used for debugging, remove before release.--------------------
        if (plantEatersOn == false)
        {
            for (int i = 0; i < plantEaterList.Count; i++)
            {
                plantEaterStartPositions.Add(plantEaterList[i].localPosition);
                Destroy(plantEaterList[i].gameObject);
            }
            plantEaterList.Clear();
        }


        // Update panels.
        GameSpeedDisplayText.text = gameSpeed.ToString();
        gamePointsText.text = gamePoints.ToString();
        worldSizeText.text = worldSize.ToString();
        foodSpawnedText.text = foodSpawned.ToString();
        PlantEatersText.text = plantEaters.ToString();
        dayCountText.text = day.ToString();
        if (slayerUpgradeUnlocked && slayerSlider.value < 10) slayerSlider.value += slowSpeedMeter;
        if (freezeUpgradeUnlocked && freezeSlider.value < 10) freezeSlider.value += slowSpeedMeter;
        if (feedUpgradeUnlocked && feedSlider.value < 10) feedSlider.value += slowSpeedMeter;
        if (caffeineUpgradeUnlocked && caffeineSlider.value < 10) caffeineSlider.value += slowSpeedMeter;



        // Check to see if creatures should be awake or asleep.
        if (timer >= (lengthOfDay / 2) && creaturesAwake == true)
        {
            creaturesAwake = false;
        }

        else if (timer < (lengthOfDay / 2) && creaturesAwake == false)
        {
            creaturesAwake = true;
        }

        // Update planet positions.
        //------------------------------------This code is used for debugging, remove before release.--------------------
        if (moveStarOn)
        {
            star1YPos = worldSize * Mathf.Sin(timerSpeedCoefficient * timer);
            star1XPos = worldSize * Mathf.Cos(timerSpeedCoefficient * timer) + worldSize / 2;
            star1Body.position = new Vector3(xPos, yPos, worldSize / 2);
        }

        //------------------------------------This code is used for debugging, remove before release.--------------------
        if (movePlanetOn)
        {
            planetZPos = worldSize * -3 * Mathf.Cos(timerSpeedCoefficient * timer);
            planetYPos = worldSize * Mathf.Cos(timerSpeedCoefficient * timer);
            planetXPos = worldSize * Mathf.Sin(timerSpeedCoefficient * timer) + worldSize / 2;
            planet1.position = new Vector3(planetXPos, planetYPos, planetZPos);
        }


        // Move the source of light, star1.
        star1.intensity = worldSize / 5;
        yPos = worldSize * Mathf.Sin(timerSpeedCoefficient * timer);
        xPos = worldSize * Mathf.Cos(timerSpeedCoefficient * timer) + worldSize / 2;
        star1.transform.position = new Vector3(xPos, yPos, worldSize / 2);


        // Take care of end of day tasks.
        if (timer >= lengthOfDay)
        {
            // Restock food at the beginning of each day.
            int currentFoodAmount = foodList.Count;
            for (int i = currentFoodAmount; i < foodSpawned; i++)
            {
                // Pick a random position.
                int randPos = Random.Range(0, foodPositions.Count);

                // Place food at position and remove its position from the available list, then place the transform in the foodList. 
                Transform f = Instantiate(food);
                f.localPosition = foodPositions[randPos];
                foodPositions.RemoveAt(randPos);
                foodList.Add(f);
            }

            // Increase the NoPlantEatersKilled by 1. This is used for the Ninja achievement.
            noPlantEatersKilledForXDays += 1;

            // Check through the list of plant eaters and destroy plant eaters that did not survive.
            for (int i = plantEaterList.Count - 1; i >= 0; i--)
            {
                if (plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten == 0)
                {
                    Destroy(plantEaterList[i].gameObject);
                    plantEaterList.RemoveAt(i);
                    plantEaters -= 1;

                    // Reset the plantEatersKilledStat used for achievements.
                    if (noPlantEatersKilledForXDays != 0) noPlantEatersKilledForXDays = 0;
                }

                // Check to see if the Gluton Achievement has been unlocked.
                else if (plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten >= 5)
                {
                    glutonImage.color = Color.white;
                    glutonImage.texture = glutonTexture;
                }
            }

            // Check through the list of meat eaters and destroy meat eaters that did not survive.
            for (int i = meatEaterList.Count - 1; i >= 0; i--)
            {
                if (meatEaterList[i].GetComponent<MeatEaterContoller>().daysSinceLastEaten >= daysUntilMeatEaterStarves)
                {
                    Destroy(meatEaterList[i].gameObject);
                    meatEaterList.RemoveAt(i);
                }

            }

            // Reset food Eaten for plant eaters.
            for (int i = 0; i < plantEaterList.Count; i++)
            {
                plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten = 0;
            }

            // Add 1 days to meat eaters starve counter.
            for (int i = 0; i < meatEaterList.Count; i++)
            {
                meatEaterList[i].GetComponent<MeatEaterContoller>().daysSinceLastEaten += 1;
            }

            // Check to see if the Ninja Achievement has been unlocked.
            if (noPlantEatersKilledForXDays == 10)
            {
                ninjaImage.color = Color.white;
                ninjaImage.texture = ninjaTexture;
            }

            // Check to see if the Survivalist Achievement has been unlocked.
            if (day == 40)
            {
                survivalistImage.color = Color.white;
                survivalistImage.texture = survivalistTexture;
            }

            // Check to see if Tutorial script needs to start Task6.
            if (GetComponent<Tutorial>().task5 && GetComponent<Tutorial>().task6 == false && foodSpawned >= 10)
            {
                GetComponent<Tutorial>().Task6();
            }


            timer = 0;
            day += 1;
            meaterEaterSpawnCounter += 1;
            meatEatersSpawnDisabled = false;
            meatEatersFrozen = false;

            // Turn Caffeine Speed off.
            caffeineSpeedOn = false;

            // At the begining of the day. Check to see if plant eaters and meat eaters have beeen fed.
            //If they have not, change their skin color to the starving color.
            // Change the color of creatures skin if they are starving and going to die.
            for (int i = 0; i < plantEaterList.Count; i++)
            {
                if (plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten == 0)
                {
                    plantEaterList[i].GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = starvingPlantEater;
                }
            }

            for (int i = 0; i < meatEaterList.Count; i++)
            {
                if (meatEaterList[i].GetComponent<MeatEaterContoller>().daysSinceLastEaten == daysUntilMeatEaterStarves)
                {
                    meatEaterList[i].GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = starvingMeatEater;
                    meatEaterList[i].GetChild(0).GetChild(2).GetComponent<Renderer>().material.color = starvingMeatEater;
                    meatEaterList[i].GetChild(0).GetChild(3).GetComponent<Renderer>().material.color = starvingMeatEater;
                }
            }
        }
    }

    // Save data to Global Control
    public void SaveData()
    {
        GlobalControl.Instance.worldSize = worldSize;
        GlobalControl.Instance.foodSpawned = foodSpawned;
        GlobalControl.Instance.plantEaters = plantEaters;
        GlobalControl.Instance.gamePoints = gamePoints;
    }

    // This section handles buttons on screen.
    public void ClickPause()
    {
        if (gamePaused == false)
        {
            gamePaused = true;
            gameSpeed = 0;
        }
    }

    public void ClickForward()
    {
        if (GetComponent<Tutorial>().task1)
        {
            // Only makes changes if the lengthOfDay = 360, meaning forward mode was occuring. 
            if (timerSpeedCoefficient == 1)
            {
                lengthOfDay = 25.12f;
                timerSpeedCoefficient = .25f;
                timer *= 4;
                gameSpeed = 1;
            }

            // only makes changes when this button is pushed while game is paused.
            if (gamePaused == true)
            {
                gamePaused = false;
            }

            // Update the plant eater's and meat eater's speed to match fast forward.
            plantEaterSpeed = .5f;
            caffeinePlantEaterSpeed = 1.5f;
            meatEaterSpeed = .5f;
            slowSpeedMeter = .001f;
            medSpeedMeter = .01f;

        }
        
    }

    public void ClickFastForward()
    {
        // Only make changes if the lengthOFDay = 1440, meaning fast forward was occuring. 
        if (timerSpeedCoefficient == .25)
        {
            lengthOfDay = 6.28f;
            timerSpeedCoefficient = 1f;
            timer /= 4;
            gameSpeed = 4;
        }

        // only makes changes when this button is pushed while game is paused.
        if (gamePaused == true)
        {
            gamePaused = false;
        }

        // Update the plant eater's and meat eater's speed to match fast forward.
        plantEaterSpeed = 2f;
        caffeinePlantEaterSpeed = 6f;
        meatEaterSpeed = 2f;
        slowSpeedMeter = .004f;
        medSpeedMeter = .04f;

    }

    public void ClickFoodSpawned()
    {
        if (gamePoints > 0 && foodPositions.Count > 0 && GetComponent<Tutorial>().task5)
        {
            gamePoints -= 1;
            foodSpawned += 1;
        }
    }

    public void ClickWorldSize()
    {
        if (gamePoints > 0 && worldSize < worldSizeLimit && GetComponent<Tutorial>().task6)
        {
            gamePoints -= 1;
            worldSize += 1;
            GameObject.Find("Grid").GetComponent<TutCreateGrid>().UpdateGrid();
            GameObject.Find("Grid").GetComponent<TutCreateGrid>().UpdateCameraPos();
        }

        // Check to see if the Land Owner achievement is unlocked.
        if (worldSize >= 20)
        {
            landOwnerImage.color = Color.white;
            landOwnerImage.texture = landOwnerTexture;
        }


    }

    public void ClickPlantEaters()
    {
        if (gamePoints > 0 && GetComponent<Tutorial>().task2)
        {
            gamePoints -= 1;
            plantEaters += 1;
        }
    }

    public void ClickSlayerUpgrade()
    {
        if (slayerUpgradeUnlocked && slayerSlider.value > 9)
        {
            // Kill meat eaters.
            for (int i = 0; i < meatEaterList.Count; i++)
            {
                Destroy(meatEaterList[i].gameObject);
            }
            meatEaterList.Clear();

            // Reset slider.
            slayerSlider.value = 0f;

            // Set the bool to false so that meat eaters do not come back immediately.
            meatEatersSpawnDisabled = true;
        }

        else if (slayerUpgradeUnlocked == false && gamePoints >= 30)
        {
            // Make the cost text invisible.
            slayerSliderText.text = "";

            // Kill meat eaters.
            for (int i = 0; i < meatEaterList.Count; i++)
            {
                Destroy(meatEaterList[i].gameObject);
            }
            meatEaterList.Clear();

            slayerSlider.gameObject.SetActive(true);
            slayerUpgradeUnlocked = true;
            gamePoints -= 30;

            // Set the bool to false so that meat eaters do not come back immediately.
            meatEatersSpawnDisabled = true;

            // Check to see if all achievements are unlocked.
            if (caffeineUpgradeUnlocked && feedUpgradeUnlocked && freezeUpgradeUnlocked && slayerUpgradeUnlocked)
            {
                unlockedImage.color = Color.white;
                unlockedImage.texture = unlockedTexture;
            }
        }

    }

    public void ClickFreezeUpgrade()
    {
        if (freezeUpgradeUnlocked && freezeSlider.value > 9)
        {
            // Set the freeze bool for meat eaters.
            meatEatersFrozen = true;

            // Reset slider.
            freezeSlider.value = 0f;
        }

        else if (freezeUpgradeUnlocked == false && gamePoints >= 20)
        {
            // Make the cost text invisible.
            freezeSliderText.text = "";

            // Set the freeze bool for meat eaters.
            meatEatersFrozen = true;

            freezeSlider.gameObject.SetActive(true);
            freezeUpgradeUnlocked = true;
            gamePoints -= 20;

            // Check to see if all achievements are unlocked.
            if (caffeineUpgradeUnlocked && feedUpgradeUnlocked && freezeUpgradeUnlocked && slayerUpgradeUnlocked)
            {
                unlockedImage.color = Color.white;
                unlockedImage.texture = unlockedTexture;
            }

        }
    }

    public void ClickFeedUpgrade()
    {
        if (feedUpgradeUnlocked && feedSlider.value > 9)
        {
            // Reset slider.
            feedSlider.value = 0f;

            // Feed all plant eaters and change their skin color.
            for (int i = 0; i < plantEaterList.Count; i++)
            {
                plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten += 1;
                plantEaterList[i].GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = FedPlantEater;
            }

        }

        else if (feedUpgradeUnlocked == false && gamePoints >= 20)
        {
            // Make the cost text invisible.
            feedSliderText.text = "";

            // Feed all plant eaters and change their skin color.
            for (int i = 0; i < plantEaterList.Count; i++)
            {
                plantEaterList[i].GetComponent<TutPlantEaterContoller>().foodEaten += 1;
                plantEaterList[i].GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = FedPlantEater;
            }

            feedSlider.gameObject.SetActive(true);
            feedUpgradeUnlocked = true;
            gamePoints -= 20;

            // Check to see if all achievements are unlocked.
            if (caffeineUpgradeUnlocked && feedUpgradeUnlocked && freezeUpgradeUnlocked && slayerUpgradeUnlocked)
            {
                unlockedImage.color = Color.white;
                unlockedImage.texture = unlockedTexture;
            }

        }

    }

    public void ClickCaffeineUpgrade()
    {
        if (caffeineUpgradeUnlocked && caffeineSlider.value > 9)
        {
            // Reset slider.
            caffeineSlider.value = 0f;

            // Set caffeine speed on;
            caffeineSpeedOn = true;

        }

        else if (caffeineUpgradeUnlocked == false && gamePoints >= 20)
        {
            // Make the cost text invisible.
            caffeineSliderText.text = "";

            caffeineSlider.gameObject.SetActive(true);
            caffeineUpgradeUnlocked = true;
            gamePoints -= 10;

            // Set caffeine speed on;
            caffeineSpeedOn = true;

            // Check to see if all achievements are unlocked.
            if (caffeineUpgradeUnlocked && feedUpgradeUnlocked && freezeUpgradeUnlocked && slayerUpgradeUnlocked)
            {
                unlockedImage.color = Color.white;
                unlockedImage.texture = unlockedTexture;
            }

        }
    }
}