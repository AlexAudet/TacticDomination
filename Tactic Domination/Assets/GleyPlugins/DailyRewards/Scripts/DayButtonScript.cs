namespace GleyDailyRewards
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class DayButtonScript : MonoBehaviour
    {
        bool isAvailable;
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI rewardValue;

       //public Image dayBg;
       //public Sprite claimedSprite;
       //public Sprite currentSprite;
       //public Sprite availableSprite;
       //public Sprite lockedSprite;

        public GameObject claimed, current, available, locked;
        [Space(20)] 
        public Transform cardHolder;
        public GameObject CoinSmall, CoinMedium, CoinBig;
        public GameObject GemSmall, GemMedium, GemBig;

        int dayNumber;
        int reward;
        string currentKey;
        RewardType type;
        MinionData minionData;

        /// <summary>
        /// Setup each day button
        /// </summary>
        /// <param name="dayNumber">current day number</param>
        /// <param name="rewardSprite">button sprite</param>
        /// <param name="rewardValue">reward value</param>
        /// <param name="currentDay">current active day</param>
        /// <param name="timeExpired">true if timer expired</param>
        public void Initialize(RewardType rewardType, int dayNumber, int rewardValue, int currentDay, bool timeExpired, ValueFormatterFunction valueFormatterFunction, string key = null)
        {
            dayText.text = "Day " + dayNumber.ToString();
            bool formattedUsingFormatterFunction = false;
            if (valueFormatterFunction != null)
            {
                try
                {
                    this.rewardValue.text = valueFormatterFunction(rewardValue);
                    formattedUsingFormatterFunction = true;
                }
                catch (System.Exception)
                {
                }
            }
            if (!formattedUsingFormatterFunction)
            {
                this.rewardValue.text = rewardValue.ToString();
              
            }   

            this.dayNumber = dayNumber;
            this.type = rewardType;
            reward = rewardValue;
            currentKey = key;

            CoinSmall.SetActive(type == RewardType.Coin && reward <= 100);
            CoinMedium.SetActive(type == RewardType.Coin && reward > 100 && reward < 500);
            CoinBig.SetActive(type == RewardType.Coin && reward >= 500);
            GemSmall.SetActive(type == RewardType.Gem && reward <= 10);
            GemMedium.SetActive(type == RewardType.Gem && reward > 10 && reward < 50);
            GemBig.SetActive(type == RewardType.Gem && reward >= 50);
            //cardHolder.gameObject.SetActive(type == RewardType.Minion);
            if (rewardType == RewardType.Minion)
            {
                foreach (var item in MinionInventory.Instance.minionInventory)
                    if (item.minionKey == currentKey)
                        this.minionData = item;
            }

            Refresh(currentDay, timeExpired);
        }


        /// <summary>
        /// Refresh button if required
        /// </summary>
        /// <param name="currentDay"></param>
        /// <param name="timeExpired"></param>
        public void Refresh(int currentDay, bool timeExpired)
        {
            //Day qui a ete reclammer
            if (dayNumber - 1 < currentDay)
            {
                dayText.transform.parent.gameObject.SetActive(false);
                isAvailable = false;
                claimed.SetActive(true);
                locked.SetActive(false);
                current.SetActive(false);       
                available.SetActive(false);
            }

            if (dayNumber - 1 == currentDay)
            {
                //Day a reclammer
                if (timeExpired == true)
                {
                    isAvailable = true;
                    available.SetActive(true);
                    current.SetActive(false);
                    locked.SetActive(false);
                    claimed.SetActive(false);
                    dayText.transform.parent.gameObject.SetActive(true);
                }
                else //Day en recharge
                {
                    isAvailable = false;
                    current.SetActive(true);
                    claimed.SetActive(false);
                    available.SetActive(false); 
                    locked.SetActive(false);
                    dayText.transform.parent.gameObject.SetActive(true);
                }
            }

            //DayLocked
            if (dayNumber - 1 > currentDay)
            {
                isAvailable = false;
                locked.SetActive(true);
                current.SetActive(false);
                claimed.SetActive(false);
                available.SetActive(false);
                dayText.transform.parent.gameObject.SetActive(true);
            }
        }


        /// <summary>
        /// Called when a day button is clicked
        /// </summary>
        public void ButtonClicked()
        {
            if (isAvailable)
            {
                CalendarManager.Instance.ButtonClick(dayNumber, reward, type, currentKey);
            }
        }
    }
}
