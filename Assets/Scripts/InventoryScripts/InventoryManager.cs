using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPref;             // посилання на предмет

    public GameObject inventoryPanel;       // посилання на об'єкт панель інвентаря
    // public GameObject chestPanel;           // посилання на об'єкт панель сундука
    //
    // public GameObject descriptionPanel;     // посилання на панельку опису предмета

    public GameObject inventoryContent;     // посилання на місце спавну предмету в інвентарі
   //  public GameObject chestContent;         // посилання на місце спавну предмету в сундук панель

    public ItemData[] items;                               // тут ми будемо зберігати ВСІ МОЖЛИВІ предмети гри

    public List<GameObject> inventorySlots = new List<GameObject>();
    // public List<GameObject> currentChestSlots = new List<GameObject>();

    private void Awake()
    {
        inventoryPanel = GameObject.Find("Inventory");        // коричневим йде ім'я з ієрархії
        // chestPanel = GameObject.Find("Panel_Chest");
        inventoryContent = GameObject.Find("InventoryContent");
        // chestContent = GameObject.Find("Content_Chest");
        //
        // descriptionPanel = GameObject.Find("Panel_Description");
    }
    // Start is called before the first frame update
    void Start()
    {
        inventoryPanel.SetActive(false);
        // chestPanel.SetActive(false);
        // descriptionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateItem(int itemID, List<ItemData> itemsList)
    {
        // створюємо предмет на основі списку, який отримуємо під час виклику цього методу
        ItemData item = new ItemData(items[itemID].name,
                                        items[itemID].id,
                                        items[itemID].count,
                                        items[itemID].description,
                                        items[itemID].isUniq);

        if (!item.isUniq && itemsList.Count > 0)                         // предмет НЕ унікальний і в списку є
        {
            for (int i = 0; i < itemsList.Count; i++)                    // перебираємо список і шукаємо збіг
            {
                if (item.id == itemsList[i].id)                          // якщо збіг знайдено та ID співпадають
                {
                    itemsList[i].count += item.count;                            // додаємо в список +1 предмет
                    break;
                }
                else if (i == itemsList.Count - 1)                       // якщо збігів не знеайдено
                {
                    itemsList.Add(item);                                // створюємо новий предмет в списку
                    break;
                }
            }
        }
        else if (item.isUniq || (!item.isUniq && itemsList.Count == 0))      // предмет унікальний або такого предмету в списку нема
        {
            itemsList.Add(item);                                        // створюємо новий предмет в списку
        }
    }


    public void InstantiatingItem(ItemData item, Transform parent, List<GameObject> itemsList)
    {
        GameObject go = Instantiate(slotPref);

        go.transform.SetParent(parent.transform);
        go.AddComponent<Slot>();
        go.GetComponent<Slot>().itemData = item;

        go.transform.Find("Name").GetComponent<Text>().text = item.name;
        go.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(item.name);
        go.transform.Find("Count").GetComponent<Text>().text = item.count.ToString();

        // якщо предмет унікальний перефарбуємо колір

        itemsList.Add(go);
    }


}
