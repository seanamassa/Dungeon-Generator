# Metroidvania Dungeon Generator

A procedural level design tool built in Unity that generates grid-based dungeon layouts in real-time. This tool features a "Metroidvania" logic layer that intelligently places keys and locks to create backtracking gameplay loops, rather than just random linear paths.

## Features


* **Runtime Generation:** Instantly generate new layouts directly in the Game View without restarting.
* **Metroidvania Logic:** A toggleable mode that locks the Boss Room and hides a Key in a distant "dead-end" room, forcing player exploration.
* **Parametric Controls:**
    * **Room Count:** Scale dungeons from small 5-room encounters to massive 50-room labyrinths.
    * **Branching Factor:** Adjust the layout shape from linear corridors (Low) to sprawling, complex webs (High).
    * **Loot Density:** Control the risk/reward balance by adjusting the number of Treasure rooms.
      
![VideoProject1-ezgif com-crop](https://github.com/user-attachments/assets/1e9c241f-908f-4136-9f89-cdfb2d2e6ba5)
![demo4](https://github.com/user-attachments/assets/5a8e3a86-02ef-4ebb-8251-3d9583b4d7ec)
##  Getting Started

1.  **Clone the Repository:**
    Use GitHub Desktop for ease of use.
    Clone using this link:
    (https://github.com/seanamassa/Dungeon-Generator.git)
3.  **Open in Unity:**
    * Launch Unity Hub and add the project folder.
    * Recommended Version: **Unity 6** (or 2022.3+).
4.  **Run the Generator:**
    * Open the scene located in `Scenes/SampleScene`.
    * Press the **Play** button.
    * Use the UI panel on the left to configure your dungeon and click **"Generate"**.
    * To save an image directly to the project folder press **"Save Image"**.
    * A new folder will be created within the Builds folder called **"Dungeon Screenshots"**.

## Controls & Legend

### Parameters
| Setting | Description |
| :--- | :--- |
| **Rooms** | Slider (5-50). Sets the maximum number of rooms to spawn. |
| **Loot Rooms** | Slider (0-10). Determines how many "Treasure" rooms are scattered in the map. |
| **Branching** | Slider (0.0 - 1.0). **0.0** = Linear line. **1.0** = High chance of branching paths. |
| **Lock Boss Room** | Metroidvania Toggle. If ON, guarantees a locked Boss door and a hidden Key. |

### Map Legend
The generator uses a color-coded system to identify room types:

* 🟩 **Green:** Start Room (Entry point)
* 🟥 **Red:** Boss Room (Goal/Exit)
* 🟦 **Cyan:** Key Room (Required to access Boss in Metroidvania mode)
* 🟨 **Yellow:** Loot/Treasure Room
* ⬜ **Grey:** Standard Room

### Known Limitations
* **No Loop Generation**: The current algorithm produces spanning trees (perfect mazes with no loops). This means there is always exactly one path between any two rooms, which can lead to frequent backtracking. 
* **Boss/Key Distance**: In extremely small dungeons, the "Metroidvania" logic may fail to find a distant dead end, occasionally placing the Key adjacent to the Boss or Start room.


