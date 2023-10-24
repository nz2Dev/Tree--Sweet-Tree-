using UnityEngine;
using UnityEngine.UI;

public class PopUpNotificationsDisplay : MonoBehaviour {

    [SerializeField] private PopUpNotifications notifications;
    [SerializeField] private Image emotionsImage;
    [SerializeField] private AnimationCurve emotionsPopUpScaleCurve;
    [SerializeField] private Image hintImage;
    [SerializeField] private AnimationCurve hintPopUpScaleCurve;

    private Suggestion activeSuggestion;
    private Canvas canvas;
    private RectTransform emotionsImageRect;
    private RectTransform hintImageRect;

    private bool playingEmotionSequence;
    private float playinEmotionSequenceStartTime;
    private bool playingHintSequence;
    private float playingHintSequenceStartTime;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        emotionsImageRect = emotionsImage.transform as RectTransform;
        hintImageRect = hintImage.transform as RectTransform;
        notifications.OnShowNotification += NotificationsOnShowNotification;
    }

    private void Start() {
        canvas.enabled = false;
    }

    private void NotificationsOnShowNotification(Suggestion suggestion) {        
        activeSuggestion = suggestion;
        canvas.enabled = true;

        PlayEmotionDisplay();
    }

    public void OnEmmotionClick() {
        activeSuggestion.emotionDuration = 0;
    }

    private void PlayEmotionDisplay() {
        emotionsImage.gameObject.SetActive(true);
        emotionsImage.sprite = activeSuggestion.emotion;
        hintImage.gameObject.SetActive(false);

        playingEmotionSequence = true;
        playinEmotionSequenceStartTime = Time.time;
    }

    private void PlayHintDisplay() {
        emotionsImage.gameObject.SetActive(false);
        hintImage.gameObject.SetActive(true);
        hintImage.sprite = activeSuggestion.hint;

        playingHintSequence = true;
        playingHintSequenceStartTime = Time.time;
    }

    private void Update() {
        UpdateRotation();

        if (playingEmotionSequence) {
            var emotionSequenceEndTime = playinEmotionSequenceStartTime + activeSuggestion.emotionDuration;
            if (Time.time < emotionSequenceEndTime) {
                var displayProgress = (Time.time - playinEmotionSequenceStartTime) / activeSuggestion.emotionDuration;
                var scale = emotionsPopUpScaleCurve.Evaluate(displayProgress);
                emotionsImageRect.localScale = new Vector3(scale, scale, scale);
                emotionsImageRect.GetComponent<CanvasGroup>().alpha = scale;
            } else {
                playingEmotionSequence = false;
                PlayHintDisplay();
            }
        }

        if (playingHintSequence) {
            var hintSequenceEndTime = playingHintSequenceStartTime + activeSuggestion.hintDuration;
            if (Time.time < hintSequenceEndTime) {
                var displayProgress = (Time.time - playingHintSequenceStartTime) / activeSuggestion.hintDuration;
                var scale = hintPopUpScaleCurve.Evaluate(displayProgress);
                hintImageRect.localScale = new Vector3(scale, scale, scale);
                hintImageRect.GetComponent<CanvasGroup>().alpha = scale;
            } else {
                Debug.Log("Done");
                playingHintSequence = false;
                hintImage.gameObject.SetActive(false);
                canvas.enabled = false;
            }
        }
    }

    private void UpdateRotation() {
        var camera = Camera.main;
        transform.rotation = camera.transform.rotation;
    }

}
