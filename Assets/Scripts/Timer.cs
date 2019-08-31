using System;
using System.Collections;
using UnityEngine;
// Define time denomination
public enum TimeDenomination {
    Seconds,
    Minutes,
    Hours,
    Days,
    Months
}
// Class to rotate digits based on value from WallpaperManager class
public class Timer : MonoBehaviour {
    public TimeDenomination denomination;                           // enum object for denomination
    public GameObject firstDigit, secondDigit;                      // reference to first and second place digits
    public SpriteRenderer[] facesFirst, facesSecond;                // sprites for first and second place digits
    public Sprite[] numbers;                                        // array of sprites for all digits
    public float sensitivity = 10f;                                 // how quickly the digits rotate
    private int faceIndexfirst;                                     // index of int value for first place value
    private int faceIndexSecond;                                    // index of int value for second place value
    private int i = 1, j = 1;                                       // counters
    private bool doMoveFirst = false;                               // if true move first place
    private bool doMoveSecond = false;                              // if true move second place
    int prevFirst;                                                  // hold previous value for first place
    int prevSecond;                                                 // hold previous value for second place
    // valid roatation quaternion values for digits to rotate (otherwise small errors add up)
    readonly Quaternion[] states = {
        Quaternion.identity * Quaternion.Euler(0, 0, 0),
        Quaternion.identity * Quaternion.Euler(-90, 0, 0),
        Quaternion.identity * Quaternion.Euler(-180, 0, 0),
        Quaternion.identity * Quaternion.Euler(-270, 0, 0)
    };
    // When app loads, initialize with values
    void Awake() {
        int firstPlace = 0, secondPlace = 0;
        // each rotating digit has this script attached to it, based on which digit it is, set initial value
        int temp = WallpaperManager.TimerValue(denomination);
        firstPlace = temp % 10;
        secondPlace = temp / 10;
        prevFirst = firstPlace;
        prevSecond = secondPlace;
        faceIndexfirst = 0;
        faceIndexSecond = 0;
        facesFirst[faceIndexfirst].sprite = numbers[0];
        facesSecond[faceIndexSecond].sprite = numbers[0];
        // counters to keep track of which face of rotating digit cube is at the front
        i = 0;
        j = 0;
        for (int q = 0; q < 4; q++) {
            if (q != j) {
                facesSecond[q].sprite = null;
            }
        }
        for (int q = 0; q < 4; q++) {
            if (q != i) {
                facesFirst[q].sprite = null;
            }
        }
        Application.targetFrameRate = 60;
    }
    void Update() {
        // reset face index if outside of defined four
        if (faceIndexfirst > 3) {
            faceIndexfirst = 0;
        }
        if (faceIndexSecond > 3) {
            faceIndexSecond = 0;
        }
        // variable to store calculated int of place value and then apply at the end
        int val1, val2;
        // temp variable used for calculations
        int temp;
        // calcualtion is done for specified denomination
        temp = WallpaperManager.TimerValue(denomination);
        val1 = temp % 10;
        val2 = temp / 10;

        // set digits based on int
        facesFirst[faceIndexfirst].sprite = numbers[val1];
        facesSecond[faceIndexSecond].sprite = numbers[val2];
        // if time/int value has changed and not equal to current digit value, update and set boolean to rotate the digit true for first place value
        if (prevFirst != val1) {
            doMoveFirst = true;
            prevFirst = val1;
        }
        // if booelan is true, start roating the cube for second digit
        if (doMoveFirst) {
            StartCoroutine(RotateFirst(firstDigit));
            doMoveFirst = false;
        }
        // if time/int value has changed and not equal to current digit value, update and set boolean to rotate the digit true for second place value
        if (prevSecond != val2) {
            doMoveSecond = true;
            prevSecond = val2;
        }
        // if booelan is true, start rotating the cube for second digit 
        if (doMoveSecond) {
            StartCoroutine(RotateSecond(secondDigit));
            doMoveSecond = false;
        }
    }
    // Coroutine to rotate the digits, first place value
    private IEnumerator RotateFirst(GameObject obj) {
        i++;
        ++faceIndexfirst;
        if (i > 0 && i % 4 == 0) {
            obj.transform.rotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
            i = 0;
            faceIndexfirst = 0;
        }
        for (int q = 0; q < 4; q++) {
            if (q != i) {
                facesFirst[q].sprite = null;
            }
        }
        while (obj.transform.rotation != states[i]) {
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, states[i], sensitivity);
            yield return new WaitForSeconds(0);
        }
    }
    // Coroutine to rotate the digits, second place value
    private IEnumerator RotateSecond(GameObject obj) {
        j++;
        ++faceIndexSecond;
        if (j > 0 && j % 4 == 0) {
            j = 0;
            faceIndexSecond = 0;
        }
        for (int q = 0; q < 4; q++) {
            if (q != j) {
                facesSecond[q].sprite = null;
            }
        }
        while (obj.transform.rotation != states[j]) {
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, states[j], sensitivity);
            yield return new WaitForSeconds(0);
        }
    }
}
//  old timer calculation
//awake script
//switch (denomination) {
//            case TimeDenomination.Seconds:
//                firstPlace = DateTime.Now.Second % 10;
//                secondPlace = DateTime.Now.Second / 10;
//                break;
//            case TimeDenomination.Minutes:
//                firstPlace = DateTime.Now.Minute % 10;
//                secondPlace = DateTime.Now.Minute / 10;
//                break;
//            case TimeDenomination.Hours:
//                firstPlace = DateTime.Now.Hour % 10;
//                secondPlace = DateTime.Now.Hour / 10;
//                break;
//            case TimeDenomination.Days:
//                firstPlace = DateTime.Now.Day % 10;
//                secondPlace = DateTime.Now.Day / 10;
//                break;
//            case TimeDenomination.Months:
//                firstPlace = DateTime.Now.Month % 10;
//                secondPlace = DateTime.Now.Month / 10;
//                break;
//        }
//value of current time stored in variables, seperate for each place value of denominations
//int m1, d2, d1, h2, h1, n2, n1, s2, s1, val1, val2;
//m1 = DateTime.Now.Month % 10;
//        d2 = DateTime.Now.Day / 10;
//        d1 = DateTime.Now.Day % 10;
//        h2 = DateTime.Now.Hour / 10;
//        h1 = DateTime.Now.Hour % 10;
//        n2 = DateTime.Now.Minute / 10;
//        n1 = DateTime.Now.Minute % 10;
//        s2 = DateTime.Now.Second / 10;
//        s1 = DateTime.Now.Second % 10;
//switch (denomination) {
//    case TimeDenomination.Seconds:
//        // calculated by subtracting current time from 60 and splitting value into digits
//        temp = 60 - (s2 * 10 + s1);
//        if (temp == 60) {
//            temp = 0;
//        }
//        val1 = temp % 10;
//        val2 = temp / 10;
//        break;
//    case TimeDenomination.Minutes:
//        // calculated by subtracting current time from 59 (not 60 cause carry overs) and splitting value into digits
//        temp = 59 - (n2 * 10 + n1);
//        val1 = temp % 10;
//        val2 = temp / 10;
//        break;
//    case TimeDenomination.Hours:
//        // calculated by subtracting current time from 23 (not 24 cause carry overs) and splitting value into digits
//        temp = 23 - (h2 * 10 + h1);
//        val1 = temp % 10;
//        val2 = temp / 10;
//        break;
//    case TimeDenomination.Days:
//        // if day of month is  after 16th, subtract from a whole month (29 cause carry over) and add 16, else subtract from 15 (cause carry over)
//        if ((d2 * 10 + d1) > 16) {
//            temp = 16 + (29 - (d2 * 10 + d1));
//            val1 = temp % 10;
//            val2 = temp / 10;
//        } else {
//            temp = 15 - (d2 * 10 + d1);
//            val1 = temp % 10;
//            val2 = temp / 10;
//        }
//        break;
//    case TimeDenomination.Months:
//        // if month is after april, it means it's still 2019 so check date, if after 16, means one month is still left to be adjusted in calculation
//        if (m1 > 4) {
//            if ((d2 * 10 + d1) > 16) {
//                // 15 cause 12 months + 4th month of april - 1 carry over
//                val1 = 15 - m1;
//            } else {
//                // carry over month used, 16
//                val1 = 16 - m1;
//            }
//        }
//        // if month is less than 4, means it is 2020
//        else if (m1 <= 4) {
//            if ((d2 * 10 + d1) == 16) {
//                val1 = 3 - m1;
//            } else {
//                val1 = 4 - m1;
//            }
//        }
//        break;
//}
