using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BorderCreator: MonoBehaviour
{
    public GameObject bordersPrefab;
    public GameObject pointPrefab;

    public Transform bordersRoot;
    public BorderPoint[] points;
    public ArrayList borders;

    public bool isChanged = false;
    public bool createPoint = false;
    public bool clearRoot = false;
    public float updateInterval = 5;

    private float _updateTime = 0;

    void Awake()
    {
        if (Application.isPlaying) {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (Application.isPlaying) {
            enabled = false;
            return;
        }

        if (clearRoot) {
            clearRoot = false;
            int i;
            for (i = bordersRoot.transform.childCount - 1; i >= 0; i--) {
                GameObject.DestroyImmediate(bordersRoot.transform.GetChild(i).gameObject);
            }
        }

        if (createPoint) {
            createPoint = false;
            createNewPoint();
        }

        _updateTime += 1;
        if (_updateTime > updateInterval) {
            isChanged = true;
            _updateTime = 0;
        }

        if (isChanged) {
            isChanged = false;
            updateBorders();
        }
    }

    void updateBorders()
    {
        int i;
        if (borders == null) {
            borders = new ArrayList();
        }

        for (i = borders.Count - 1; i >= 0; i--) {
            if (borders[i] == null || (borders[i] as GameObject) == null) {
                borders.RemoveAt(i);
            }
        }

        if (borders.Count > 0 && borders.Count >= points.Length) {
            for (i = borders.Count - 1; i >= points.Length - 1; i--) {
                GameObject.DestroyImmediate(borders[i] as GameObject);
                borders.RemoveAt(i);
            }
        }

        if (borders.Count < points.Length - 1) {
            i = borders.Count;
            for (; i < points.Length - 1; i++) {
                GameObject gm = (GameObject)Instantiate(bordersPrefab);
                gm.transform.parent = bordersRoot;
                borders.Add(gm);
            }
        }

        updateTransforms();
    }

    void updateTransforms()
    {
        int i;
        for (i = 0; i < borders.Count; i++) {
            if (i + 1 < points.Length && points[i] != null && points[i + 1] != null &&
                points[i].point != null && points[i + 1].point != null
            ) {
                GameObject border = borders[i] as GameObject;
                Vector3 start = points[i].point.position;
                Vector3 end = points[i + 1].point.position;
                end.x += points[i + 1].offset;

                border.transform.position = (start + end) * 0.5f;
                border.transform.LookAt(end);
                border.transform.localScale = new Vector3(10, 1000, (start - end).magnitude);
            }
        }
    }

    void createNewPoint()
    {
        BorderPoint[] newPoints = new BorderPoint[points.Length + 1];
        int i;
        for (i = 0; i < points.Length; i++) {
            newPoints[i] = points[i];
        }
        points = newPoints;

        BorderPoint point = new BorderPoint();
        point.point = ((GameObject)Instantiate(pointPrefab)).transform;

        if (points.Length > 1) {
            point.point.parent = points[points.Length - 2].point.parent;
            point.point.position = points[points.Length - 2].point.position;
        }
        point.point.gameObject.name = "pt" + points.Length;

        points[points.Length - 1] = point;
#if UNITY_EDITOR
        UnityEditor.Selection.activeObject = point.point.gameObject;
#endif
    }
}

[System.Serializable]
public class BorderPoint
{
    public Transform point;
    public float offset = 0;
}