# Metroidvania Dungeon Generator

A procedural level design tool built in Unity that generates grid-based dungeon layouts in real-time. This tool features a "Metroidvania" logic layer that intelligently places keys and locks to create backtracking gameplay loops, rather than just random linear paths.

## Features
![VideoProject1-ezgif com-crop](https://github.com/user-attachments/assets/1e9c241f-908f-4136-9f89-cdfb2d2e6ba5)
![demo2](https://github.com/user-attachments/assets/a4f2604f-882e-4b0b-9209-054dc76bde89)


* **Runtime Generation:** Instantly generate new layouts directly in the Game View without restarting.
* **Metroidvania Logic:** A toggleable mode that locks the Boss Room and hides a Key in a distant "dead-end" room, forcing player exploration.
* **Parametric Controls:**
    * **Room Count:** Scale dungeons from small 5-room encounters to massive 50-room labyrinths.
    * **Branching Factor:** Adjust the layout shape from linear corridors (Low) to sprawling, complex webs (High).
    * **Loot Density:** Control the risk/reward balance by adjusting the number of Treasure rooms.
* **Auto-Fit Camera:** The camera automatically zooms and centers to keep the entire dungeon in frame, regardless of size.
* **Visual Debugging:** Color-coded rooms and connections for quick analysis of level flow.


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
| **Metroidvania** | Toggle. If ON, guarantees a locked Boss door and a hidden Key. |

### Map Legend
The generator uses a color-coded system to identify room types:

* 🟩 **Green:** Start Room (Entry point)
* 🟥 **Red:** Boss Room (Goal/Exit)
* 🟦 **Cyan:** Key Room (Required to access Boss in Metroidvania mode)
* 🟨 **Yellow:** Loot/Treasure Room
* ⬜ **Grey:** Standard Room


##  Author Sean Massa
