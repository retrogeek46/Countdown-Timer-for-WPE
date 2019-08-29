using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class WallpaperManager : MonoBehaviour {
    public Sprite[] wallpapers;                                                 // array containing refrences to all wallpaper images
    public SpriteRenderer wallpaperHolder;                                      // sprite renderer that shows current wallpaper
    public TimeDenomination denomination;                                       // denomination object to choose what time denomination is currently chosen
    public int duration;                                                        // duration of chosen denomination after which wallpaper should swicth
    public VideoPlayer video;                                                   // initial cyberpunk 2077 logo video
    bool videoHasStopped;                                                       // boolean to check if video has stopped
    private int wallpaperIndex;                                                 // int value of currently playing wallpaper
    int now, prev;                                                              // varaibles to hold time elapsed for switching wallpaper based on denomination and duration
    public Slider transparencySlider;                                           // how transparent the wallaper should be
    public GameObject settingsPanel;                                            // settings panel at upper right corner
    public SpriteRenderer[] digits;                                             // change color of digits from white and gray
    private Color32 grey = new Color(0.196f, 0.196f, 0.196f, 1);                // value/variable of grey color
    public Toggle themeToggle;                                                  // Toggle button reference to change theme (white/gray color)
    public int currentTheme;                                                    // current value of theme button
    bool doChange;                                                              // if theme/color is to be changed 

    void Awake() {
        // initialize values of variables
        doChange = false;
        currentTheme = PlayerPrefs.GetInt("theme", 0);
        ChangeTheme(currentTheme);
        transparencySlider.value = PlayerPrefs.GetFloat("transparency", 0.804f);
        transparencySlider.onValueChanged.AddListener(delegate { CheckValueChange(); });
        wallpaperIndex = PlayerPrefs.GetInt("wallpaperIndex", 0);
        ChangeWallpaper(wallpaperIndex);
        videoHasStopped = false;
        switch (denomination) {
            case TimeDenomination.Seconds:
                now = DateTime.Now.Second;
                break;
            case TimeDenomination.Minutes:
                now = DateTime.Now.Minute;
                break;
            case TimeDenomination.Hours:
                now = DateTime.Now.Hour;
                break;
            case TimeDenomination.Days:
                now = DateTime.Now.Day;
                break;
            case TimeDenomination.Months:
                now = DateTime.Now.Month;
                break;
        }
        prev = now;
    }
    void Update() {
        // if video playaback is complete
        if (!video.isPlaying && !videoHasStopped) {
            videoHasStopped = true;
            video.gameObject.SetActive(false);
        }
        // if duration is not 0, enable switching of wallpapers
        if (duration != 0) {
            switch (denomination) {
                case TimeDenomination.Seconds:
                    now = DateTime.Now.Second;
                    break;
                case TimeDenomination.Minutes:
                    now = DateTime.Now.Minute;
                    break;
                case TimeDenomination.Hours:
                    now = DateTime.Now.Hour;
                    break;
                case TimeDenomination.Days:
                    now = DateTime.Now.Day;
                    break;
                case TimeDenomination.Months:
                    now = DateTime.Now.Month;
                    break;
            }
            // when time is up, call function to switch wallpapers
            if ( Math.Abs(now-prev) >= duration) {
                prev = now;
                ChangeWallpaper();
            }
        }
        // if theme is to be changed
        if (doChange) {
            doChange = false;
            if (themeToggle.isOn == false) {
                for (int i = 0; i < 45; i++) {
                    digits[i].color = Color.white;
                }
            } else {
                for (int i = 0; i < 45; i++) {
                    digits[i].color = grey;
                }
            }
        }
    }
    // change transparency
    public void CheckValueChange() {
        wallpaperHolder.color = new Color(wallpaperHolder.color.r, wallpaperHolder.color.g, wallpaperHolder.color.b, transparencySlider.value);
    }
    // change wallpaper being displayed sequentially (for automatic change based on timer from duration seleced by user)
    public void ChangeWallpaper() {
        if (wallpaperIndex >= 19) {
            wallpaperIndex = 0;
        }
        wallpaperHolder.sprite = wallpapers[wallpaperIndex++];
        PlayerPrefs.SetInt("wallpaperIndex", wallpaperIndex);
        PlayerPrefs.Save();
    }
    // change wallpaper based on thumbnail chosen by user
    public void ChangeWallpaper(int wallpaperID) {
        wallpaperHolder.sprite = wallpapers[wallpaperID];
        PlayerPrefs.SetInt("wallpaperIndex", wallpaperID);
        PlayerPrefs.Save();
    }
    // open settings panel when pressing the setting cog on upper right corner
    public void Settings() {
        if (settingsPanel.activeSelf == true) {

            settingsPanel.SetActive(false);
        } else {
            settingsPanel.SetActive(true);
        }
    }
    // change color of digits when theme toggle is pressed, int used when starting program
    public void ChangeTheme(int theme) {
        doChange = true;
        if (theme == 0) {
            PlayerPrefs.SetInt("theme", 0);
            themeToggle.isOn = false;
        } else {
            themeToggle.isOn = true;
            PlayerPrefs.SetInt("theme", 1);
        }
        PlayerPrefs.Save();
    }
    // change color of digits when theme toggle is pressed, bool used when pressing theme toggle
    public void ChangeTheme(bool toggle) {
        doChange = true;
        if (toggle == false) {
            themeToggle.isOn = false;
            PlayerPrefs.SetInt("theme", 0);
            currentTheme = 0;
        } else if (toggle == true) {
            themeToggle.isOn = true;
            currentTheme = 1;
            PlayerPrefs.SetInt("theme", 1);
        }
        PlayerPrefs.Save();
    }
}
