using UnityEngine;
using UnityEngine.UI;
using TinyDungeon;

public class DungeonUI : MonoBehaviour
{
    [Header("References")]
    public DungeonGenerator generator;
    
    [Header("UI Elements")]
    public Button generateButton;
    public Slider roomSlider;
    public Slider branchingSlider;
    public Toggle metroidvaniaToggle;
    public Text statusText; 

    void Start()
    {
        roomSlider.value = generator.maxRooms;
        branchingSlider.value = generator.branchingFactor;
        metroidvaniaToggle.isOn = generator.useMetroidvaniaLogic;
        generateButton.onClick.AddListener(OnGenerateClicked);
        roomSlider.onValueChanged.AddListener(OnSettingsChanged);
        branchingSlider.onValueChanged.AddListener(OnSettingsChanged);
        metroidvaniaToggle.onValueChanged.AddListener(OnToggleChanged);

        UpdateStatusLabel();
    }
    
    void OnSettingsChanged(float value)
    {
        generator.maxRooms = Mathf.RoundToInt(roomSlider.value);
        generator.branchingFactor = branchingSlider.value;
        UpdateStatusLabel();
    }

    void OnToggleChanged(bool value)
    {
        generator.useMetroidvaniaLogic = value;
    }

    void OnGenerateClicked()
    {
        generator.GenerateDungeon();
        UpdateStatusLabel();
    }

    void UpdateStatusLabel()
    {
        if (statusText != null)
        {
            statusText.text = $"Rooms: {generator.maxRooms} | Branch: {generator.branchingFactor:F2}";
        }
    }
}

