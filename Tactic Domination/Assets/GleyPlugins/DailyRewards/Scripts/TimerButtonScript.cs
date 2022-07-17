namespace GleyDailyRewards
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class TimerButtonScript : MonoBehaviour
    {
        public TimerButtonIDs buttonID;
        public Button buttonScript;
        public TextMeshProUGUI buttonText;
        public Image iconChest;
        public GameObject adsIcon;
        public TextMeshProUGUI openCostText;

        private string completeText;
        private float currentTime;
        private bool initialized;
        public LootChestProperty chest;

        const float refreshTime = 0.3f;

        void Awake()
        {
            ClearLootButton();
        }

        public void InitializeLootButton(LootChestProperty chestData, string savedTime = null)
        {
            ClearLootButton();
            chest = chestData;
            TimerButtonManager.Instance.Initialize(buttonID, InitializationComplete, chestData, savedTime : savedTime);
            ShowLootChestButton(chestData);
        }

        public void ClearLootButton()
        {
            chest = null;
            buttonText.transform.parent.gameObject.SetActive(false);
            iconChest.transform.parent.gameObject.SetActive(false);
            openCostText.transform.parent.gameObject.SetActive(false);
            adsIcon.SetActive(false);
        }

        public void ShowLootChestButton(LootChestProperty chestData)
        {
            buttonText.transform.parent.gameObject.SetActive(true);
            iconChest.transform.parent.gameObject.SetActive(true);
            openCostText.transform.parent.gameObject.SetActive(true);

            if(chestData.chestSprite != null)
                iconChest.sprite = chestData.chestSprite;
        }


        /// <summary>
        /// Setup the button
        /// </summary>
        /// <param name="remainingTime">time until ready</param>
        /// <param name="interactable">is button clickable</param>
        /// <param name="completeText">the text that appears after timer is done</param>
        private void InitializationComplete(string remainingTime, bool interactable, string completeText)
        {
            this.completeText = completeText;
            buttonText.text = remainingTime;
            buttonScript.interactable = interactable;
            RefreshButton();
        }


        /// <summary>
        /// refresh button text
        /// </summary>
        void Update()
        {
            if (initialized)
            {
                currentTime += Time.deltaTime;
                if (currentTime > refreshTime)
                {
                    currentTime = 0;
                    RefreshButton();
                }
            }
        }


        /// <summary>
        /// update button appearance
        /// </summary>
        private void RefreshButton()
        {
            buttonText.text = TimerButtonManager.Instance.GetRemainingTime(buttonID);

            float gemCost = (ChestLootManager.instance.GemAmountPerHour * TimerButtonManager.Instance.GetTimeLeft(buttonID).x) 
                + (TimerButtonManager.Instance.GetTimeLeft(buttonID).y * ChestLootManager.instance.GemAmountPerHour) / 60;

            if (gemCost < ChestLootManager.instance.GemAmountPerHour)
                gemCost = ChestLootManager.instance.GemAmountPerHour;

            openCostText.text = Mathf.FloorToInt(gemCost).ToString();

            if (TimerButtonManager.Instance.TimeExpired(buttonID))
            {
                buttonText.text = completeText;
                buttonScript.interactable = true;
                initialized = false;
            }
            else
            {
                initialized = true;
            }
        }

        
        /// <summary>
        /// Listener triggered when button is clicked
        /// </summary>
        public void RewardButtonClicked()
        {
            if (chest != null)
                TimerButtonManager.Instance.ButtonClicked(buttonID, ClickResult);
            else
                ChestLootManager.instance.AddChestToLootButton(this, ChestLootManager.instance.ChestData[0]);
        }


        /// <summary>
        /// Reset the button state if clicked and the reward was collected
        /// </summary>
        /// <param name="timeExpired"></param>
        private void ClickResult(bool timeExpired)
        {
            if (timeExpired)
            {
                ClearLootButton();
                //TimerButtonManager.Instance.Initialize(buttonID, InitializationComplete);
            }
        }
    }
}
