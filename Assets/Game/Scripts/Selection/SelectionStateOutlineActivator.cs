using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SelectionStateOutlineActivator : MonoBehaviour {

    [SerializeField] private SelectableObject selectable;
    [SerializeField] private Color selectionColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private AnimationCurve highlightWidthCurve;
    [SerializeField] private float highlightDuration = 0.5f;
    [SerializeField] private float highlightWidthScale = 8;

    private Outline outline;

    private bool playHighlighting;
    private float playHighlightStartTime;
    
    private bool selected;
    private bool highlighted;

    private void Awake() {
        outline = GetComponent<Outline>();
    }

    private void OnEnable() {
        selectable.OnSelectionChanged += SelectableOnSelectionChanged;
        SelectableOnSelectionChanged(selectable.IsSelected);
        selectable.OnHighlightChanged += SelectableOnHighlightChanged;
        SelectableOnHighlightChanged(selectable.IsHighlighted);
    }

    private void OnDisable() {
        selectable.OnSelectionChanged -= SelectableOnSelectionChanged;
        selectable.OnHighlightChanged -= SelectableOnHighlightChanged;
    }

    private void Start() {
        outline.enabled = false;
    }

    private void Update() {
        var baseWidth = 2f; /*default in Outline.cs*/
        if (playHighlighting) {
            var highlightEndTime = playHighlightStartTime + highlightDuration;
            if (Time.time < highlightEndTime) {
                var highlightProgress = (Time.time - playHighlightStartTime) / highlightDuration;
                var highlightWidthExtra = highlightWidthCurve.Evaluate(highlightProgress) * highlightWidthScale;
                outline.OutlineWidth = baseWidth + highlightWidthExtra;
            } else {
                playHighlighting = false;
                outline.OutlineWidth = baseWidth;
            }
        } else {
            outline.OutlineWidth = baseWidth;
        }
    }

    private void SelectableOnHighlightChanged(bool highlight) {
        this.highlighted = highlight;
        ChangeOutlineState();

        if (highlight) {
            playHighlighting = true;
            playHighlightStartTime = Time.time;
        } else {
            playHighlighting = false;
        }
    }

    private void SelectableOnSelectionChanged(bool selected) {
        this.selected = selected;
        ChangeOutlineState();
    }

    private void ChangeOutlineState() {
        outline.enabled = selected || highlighted;
        outline.OutlineColor = highlighted ? highlightColor : selectionColor;
    }

}
