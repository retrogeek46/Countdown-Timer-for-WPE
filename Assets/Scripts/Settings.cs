using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    // dropdown object for setting denomination (hour, day month etc) 
    public Dropdown denomintaionDropdown;
    // dropdown object for specifying duration of selected denomination
    public Dropdown durationDropdown;
    // refrence to WallpaperManager to  change wallpaper
    public WallpaperManager wallpaperManager;
    private void Start() {
        // Initialize wallpaper with previous settings using PlayerPrefs
        ChooseWallpaper(PlayerPrefs.GetInt("currentWallpaper", 0));
        denomintaionDropdown.value = PlayerPrefs.GetInt("denominationDropdown", 0);
        durationDropdown.value = PlayerPrefs.GetInt("durationDropdown", 0);
    }
    // change wallpaper from list availabel on upper right corner
    public void ChooseWallpaper(int wallpaperID) {
        PlayerPrefs.SetInt("currentWallpaper", wallpaperID);
        wallpaperManager.ChangeWallpaper(wallpaperID);
    }
    // Set time denomination for changeof wallpaper at regular intervals
    public void SetDenomination(int choice) {
        PlayerPrefs.SetInt("denomintaionDropdown", choice);
        //PlayerPrefs.Save();
        switch (choice) {
            case 0:
                wallpaperManager.denomination = TimeDenomination.Seconds;
                break;
            case 1:
                wallpaperManager.denomination = TimeDenomination.Minutes;
                break;
            case 2:
                wallpaperManager.denomination = TimeDenomination.Hours;
                break;
            case 3:
                wallpaperManager.denomination = TimeDenomination.Days;
                break;
            case 4:
                wallpaperManager.denomination = TimeDenomination.Months;
                break;
        }
    }
    // Set duration of selected denomination for change of wallpaper at regular intervals
    public void SetDuration(int choice) {
        PlayerPrefs.SetInt("durationDropdown", choice);
        //PlayerPrefs.Save();
        switch (choice) {
            case 0:
                wallpaperManager.duration = 0;
                break;
            case 1:
                wallpaperManager.duration = 1;
                break;
            case 2:
                wallpaperManager.duration = 5;
                break;
            case 3:
                wallpaperManager.duration = 10;
                break;
            case 4:
                wallpaperManager.duration = 20;
                break;
        }
    }
}
