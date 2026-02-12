using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using TinyDungeon;

public class DungeonUI : MonoBehaviour
{
    [Header("References")]
    public DungeonGenerator generator;
    
    [Header("Controls")]
    public Button generateButton;
    public Slider roomSlider;
    public Slider branchingSlider;
    public Slider lootSlider;
    public Toggle metroidvaniaToggle;
    public UnityEngine.UI.Button saveImageButton;

    [Header("Value Labels")]
    public TextMeshProUGUI roomValueText;  
    public TextMeshProUGUI branchingValueText;
    public TextMeshProUGUI lootValueText; 

    void Start()
    {
        if (generator != null)
        {
            roomSlider.value = generator.maxRooms;
            branchingSlider.value = generator.branchingFactor;
            lootSlider.value = generator.lootRoomCount;
            metroidvaniaToggle.isOn = generator.useMetroidvaniaLogic;
        }

        generateButton.onClick.AddListener(OnGenerateClicked);
        roomSlider.onValueChanged.AddListener(OnSettingsChanged);
        branchingSlider.onValueChanged.AddListener(OnSettingsChanged);
        lootSlider.onValueChanged.AddListener(OnSettingsChanged);
        metroidvaniaToggle.onValueChanged.AddListener(OnToggleChanged);
        if(saveImageButton != null) saveImageButton.onClick.AddListener(generator.SaveDungeonImage);

        UpdateLabels();

    }

    void OnSettingsChanged(float value)
    {
        generator.maxRooms = Mathf.RoundToInt(roomSlider.value);
        generator.branchingFactor = branchingSlider.value;
        generator.lootRoomCount = Mathf.RoundToInt(lootSlider.value);
        
        UpdateLabels();
    }

    void OnToggleChanged(bool value)
    {
        generator.useMetroidvaniaLogic = value;
    }

    void OnGenerateClicked()
    {
        generator.GenerateDungeon();
    }

void UpdateLabels()
    {
        if (roomValueText != null) roomValueText.text = generator.maxRooms.ToString();
        if (branchingValueText != null) branchingValueText.text = generator.branchingFactor.ToString("F2");
        if (lootValueText != null) lootValueText.text = generator.lootRoomCount.ToString(); 
    }
}