using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use TextMeshPro for cleaner text
using TinyDungeon;

public class DungeonUI : MonoBehaviour
{
    [Header("References")]
    public DungeonGenerator generator;
    
    [Header("Controls")]
    public Button generateButton;
    public Slider roomSlider;
    public Slider branchingSlider;
    public Toggle metroidvaniaToggle;

    [Header("Value Labels (Drag Text objects here)")]
    public TextMeshProUGUI roomValueText;  
    public TextMeshProUGUI branchingValueText; 

    void Start()
    {
        if (generator != null)
        {
            roomSlider.value = generator.maxRooms;
            branchingSlider.value = generator.branchingFactor;
            metroidvaniaToggle.isOn = generator.useMetroidvaniaLogic;
        }

        generateButton.onClick.AddListener(OnGenerateClicked);
        roomSlider.onValueChanged.AddListener(OnSettingsChanged);
        branchingSlider.onValueChanged.AddListener(OnSettingsChanged);
        metroidvaniaToggle.onValueChanged.AddListener(OnToggleChanged);

        UpdateLabels();
    }

    void OnSettingsChanged(float value)
    {
        // Update Generator values
        generator.maxRooms = Mathf.RoundToInt(roomSlider.value);
        generator.branchingFactor = branchingSlider.value;
        
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
        if (roomValueText != null) 
            roomValueText.text = generator.maxRooms.ToString();
        
        if (branchingValueText != null) 
            branchingValueText.text = generator.branchingFactor.ToString("F2");
    }
}