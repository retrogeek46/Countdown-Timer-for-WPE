using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Globalization;
// code taken from stackoverflow https://stackoverflow.com/a/9216404
public struct DateTimeSpan {
    public int Years { get; }
    public int Months { get; }
    public int Days { get; }
    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }
    public int Milliseconds { get; }
    public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds) {
        Years = years;
        Months = months;
        Days = days;
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
        Milliseconds = milliseconds;
    }
    enum Phase { Years, Months, Days, Done }
    public static DateTimeSpan CompareDates(DateTime date1, DateTime date2) {
        if (date2 < date1) {
            var sub = date1;
            date1 = date2;
            date2 = sub;
        }
        DateTime current = date1;
        int years = 0;
        int months = 0;
        int days = 0;
        Phase phase = Phase.Years;
        DateTimeSpan span = new DateTimeSpan();
        int officialDay = current.Day;
        while (phase != Phase.Done) {
            switch (phase) {
                case Phase.Years:
                    if (current.AddYears(years + 1) > date2) {
                        phase = Phase.Months;
                        current = current.AddYears(years);
                    } else {
                        years++;
                    }
                    break;
                case Phase.Months:
                    if (current.AddMonths(months + 1) > date2) {
                        phase = Phase.Days;
                        current = current.AddMonths(months);
                        if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                            current = current.AddDays(officialDay - current.Day);
                    } else {
                        months++;
                    }
                    break;
                case Phase.Days:
                    if (current.AddDays(days + 1) > date2) {
                        current = current.AddDays(days);
                        var timespan = date2 - current;
                        span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                        phase = Phase.Done;
                    } else {
                        days++;
                    }
                    break;
            }
        }
        return span;
    }
}
// Class for managing the wallpaper being shown and calculating time for the countdown timer
public class WallpaperManager : MonoBehaviour {
    public Sprite[] wallpapers;                                                 // array containing refrences to all wallpaper images
    public SpriteRenderer wallpaperHolder;                                      // sprite renderer that shows current wallpaper
    public static DateTime targetDate;                                          // date to which we are counting down in local systems format
    public TimeDenomination denomination;                                       // denomination object to choose what time denomination is currently chosen
    public int duration;                                                        // duration of chosen denomination after which wallpaper should swicth
    public VideoPlayer video;                                                   // initial cyberpunk 2077 logo video
    bool videoHasStopped;                                                       // boolean to check if video has stopped
    private int wallpaperIndex;                                                 // int value of currently playing wallpaper
    int now, prev;                                                              // varaibles to hold time elapsed for switching wallpaper based on denomination and duration
    public Slider transparencySlider;                                           // how transparent the wallaper should be
    public GameObject settingsPanel;                                            // settings panel at upper right corner
    public SpriteRenderer[] digits;                                             // change color of digits from white and gray
    public Toggle themeToggle;                                                  // Toggle button reference to change theme (white/gray color)
    public int currentTheme;                                                    // current value of theme button
    bool doChangeDigitColor;                                                    // if theme/color is to be changed 
    public InputField colorHex;                                                 // Define color used for digits using hex values
    private Color digitColor;                                                   // color of countdown timer digits
    void Awake() {
        // initialize values of variables
        targetDate = DateTime.Parse("16/4/2020 00:00:00 AM");
        video.EnableAudioTrack(0, false);
        doChangeDigitColor = false;
        currentTheme = PlayerPrefs.GetInt("theme", 0);
        transparencySlider.value = PlayerPrefs.GetFloat("transparency", 0.804f);
        CheckValueChange();
        transparencySlider.onValueChanged.AddListener(delegate { CheckValueChange(); });
        wallpaperIndex = PlayerPrefs.GetInt("wallpaperIndex", 0);
        ChangeWallpaper(wallpaperIndex);
        videoHasStopped = false;
        SwitchDenomination(denomination);
        prev = now;
        colorHex.text = PlayerPrefs.GetString("colorHex", "#ffffff");
        //ChangeDigitColor(HexToColor(colorHex.text));
    }
    void Update() {
        // if intro logo video playaback is complete
        if (!video.isPlaying && !videoHasStopped) {
            videoHasStopped = true;
            video.gameObject.SetActive(false);
        }
        // if duration is not 0, enable switching of wallpapers
        if (duration != 0) {
            SwitchDenomination(denomination);
            // when time is up, call function to switch wallpapers
            if ( Math.Abs(now-prev) >= duration) {
                prev = now;
                ChangeWallpaper();
            }
        }
    }
    // function to change color when text is changed
    public void ChangeDigitColor() {
        try {
            Color tempColor = HexToColor(colorHex.text);
            Debug.Log("valid color hex");
            PlayerPrefs.SetString("colorHex", colorHex.text);
            for (int i = 0; i < 45; i++) {
                digits[i].color = tempColor;
            }
        } catch (Exception e) {
            Debug.Log("Invalid color string passed: " + e);
            for (int i = 0; i < 45; i++) {
                digits[i].color = Color.white;
            }
        }
    }
    // function to find all int for timer
    public static int TimerValue(TimeDenomination timeDenomination) {
        DateTime now = DateTime.Now;
        DateTimeSpan span = DateTimeSpan.CompareDates(targetDate, now);
        switch (timeDenomination) {
            case TimeDenomination.Seconds:
                return span.Seconds;
            case TimeDenomination.Minutes:
                return span.Minutes;
            case TimeDenomination.Hours:
                return span.Hours;
            case TimeDenomination.Days:
                return span.Days;
            case TimeDenomination.Months:
                return span.Months;
            default:
                return span.Years;
        }
    }
    // function to set value of current time int based on denomination
    public void SwitchDenomination(TimeDenomination denominationObject) {
        switch (denominationObject) {
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
    }
    // hex to color
    public Color HexToColor(string colorcode) {
        colorcode = colorcode.TrimStart('#');
        Color col;
        col = new Color(int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
                        int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                        int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
                        1);
        return col;
    }
    // change transparency
    public void CheckValueChange() {
        wallpaperHolder.color = new Color(wallpaperHolder.color.r,wallpaperHolder.color.g, wallpaperHolder.color.b, transparencySlider.value);
        PlayerPrefs.SetFloat("transparency", transparencySlider.value);
        //PlayerPrefs.Save();
    }
    // change wallpaper being displayed sequentially (for automatic change based on timer from duration seleced by user)
    public void ChangeWallpaper() {
        if (wallpaperIndex >= 19) {
            wallpaperIndex = 0;
        }
        wallpaperHolder.sprite = wallpapers[wallpaperIndex++];
        PlayerPrefs.SetInt("wallpaperIndex", wallpaperIndex);
        //PlayerPrefs.Save();
    }
    // change wallpaper based on thumbnail chosen by user
    public void ChangeWallpaper(int wallpaperID) {
        wallpaperHolder.sprite = wallpapers[wallpaperID];
        PlayerPrefs.SetInt("wallpaperIndex", wallpaperID);
        //PlayerPrefs.Save();
    }
    // open settings panel when pressing the setting cog on upper right corner
    public void Settings() {
        if (settingsPanel.activeSelf == true) {

            settingsPanel.SetActive(false);
        } else {
            settingsPanel.SetActive(true);
        }
    }
    // save playerprefs
    private void OnApplicationQuit() {
        PlayerPrefs.Save();
    }
}
