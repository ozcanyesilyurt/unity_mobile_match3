using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Text;
using System.Reflection;

public class HierarchyCopier : OdinEditorWindow
{
	[MenuItem("Tools/Hierarchy Copier")]
	private static void OpenWindow()
	{
		GetWindow<HierarchyCopier>().Show();
	}

	[Button("Copy All GameObjects In Scene")]
	private void CopyAllGameObjects()
	{
		var allRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		StringBuilder sb = new StringBuilder();
		foreach (var go in allRoots)
		{
			SerializeGameObjectRecursive(go, sb, 0);
		}
		EditorGUIUtility.systemCopyBuffer = sb.ToString();
		Debug.Log("All GameObjects copied to clipboard!");
	}

	[Button("Copy Selected GameObject(s)")]
	private void CopySelectedGameObjects()
	{
		var selected = Selection.gameObjects;
		if (selected == null || selected.Length == 0)
		{
			Debug.LogWarning("No GameObject selected.");
			return;
		}
		StringBuilder sb = new StringBuilder();
		foreach (var go in selected)
		{
			SerializeGameObjectRecursive(go, sb, 0);
		}
		EditorGUIUtility.systemCopyBuffer = sb.ToString();
		Debug.Log("Selected GameObject(s) copied to clipboard!");
	}

	private void SerializeGameObjectRecursive(GameObject go, StringBuilder sb, int indent)
	{
		string indentStr = new string(' ', indent * 2);
		sb.AppendLine($"{indentStr}GameObject: {go.name}");
		foreach (var comp in go.GetComponents<Component>())
		{
			if (comp == null) continue;
			sb.AppendLine($"{indentStr}  Component: {comp.GetType().Name}");
			SerializeComponentFields(comp, sb, indent + 2);
		}
		foreach (Transform child in go.transform)
		{
			SerializeGameObjectRecursive(child.gameObject, sb, indent + 1);
		}
	}

	private void SerializeComponentFields(Component comp, StringBuilder sb, int indent)
	{
		string indentStr = new string(' ', indent * 2);
		
		// Special handling for RectTransform
		if (comp is RectTransform rectTransform)
		{
			sb.AppendLine($"{indentStr}anchoredPosition: {rectTransform.anchoredPosition}");
			sb.AppendLine($"{indentStr}sizeDelta: {rectTransform.sizeDelta}");
			sb.AppendLine($"{indentStr}anchorMin: {rectTransform.anchorMin}");
			sb.AppendLine($"{indentStr}anchorMax: {rectTransform.anchorMax}");
			sb.AppendLine($"{indentStr}pivot: {rectTransform.pivot}");
			sb.AppendLine($"{indentStr}localPosition: {rectTransform.localPosition}");
			sb.AppendLine($"{indentStr}localRotation: {rectTransform.localRotation}");
			sb.AppendLine($"{indentStr}localScale: {rectTransform.localScale}");
			return;
		}
		
		// Special handling for Transform (if not RectTransform)
		if (comp is Transform transform && !(comp is RectTransform))
		{
			sb.AppendLine($"{indentStr}localPosition: {transform.localPosition}");
			sb.AppendLine($"{indentStr}localRotation: {transform.localRotation}");
			sb.AppendLine($"{indentStr}localScale: {transform.localScale}");
			return;
		}
		
		var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (var field in fields)
		{
			// Only show serializable fields
			if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null) continue;
			object value = field.GetValue(comp);
			string valueStr = value == null ? "null" : value.ToString();
			sb.AppendLine($"{indentStr}{field.Name}: {valueStr}");
		}
	}
}
