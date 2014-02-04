using UnityEngine;

[AddComponentMenu("NGUI/Motiv/Drag & Drop Root")]
public class DragDropRoot : MonoBehaviour
{
	static public Transform root;

	void Awake()
    {
        root = transform;
    }
}