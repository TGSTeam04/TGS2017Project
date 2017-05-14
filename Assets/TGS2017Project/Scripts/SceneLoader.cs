// 簡易シーンローダー

// Atelier Empressのblog
// 【Unity】マルチシーンエディティングを使った簡易シーンローダー【小ネタ】
// http://atelierempress.blog.fc2.com/blog-entry-73.html

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// シーンID（シーン名称と同じにする）
/// </summary>
public enum SceneID {
	Player,
	TestStage,
}
/**
 *   マルチシーン呼び出しクラス
 */
public class SceneLoader : MonoBehaviour {
	/// <summary> 初回に読み込むシーンID </summary>
	[SerializeField]
	SceneID loadScene;


	//------------------------------------------------------------------------------------
	// 指定シーンのロード
	public void LoadScene() {
		OpenEditorScene(loadScene.ToString());
	}
	//------------------------------------------------------------------------------------
	// 指定シーンの解放
	public void RemoveScene() {
		CloseEditorScene(loadScene.ToString());
	}

	//------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------
	/// <summary>
	/// Editor上でのシーンの読み出し（シーンID版）
	/// ※ SceneNameManager ありきの読み出し
	/// </summary>
	/// <param name="sceneName">シーン名称
	void OpenEditorScene(string sceneName) {
#if UNITY_EDITOR
		// AssetDatabaseでシーンファイルを探索
		foreach (var guid in AssetDatabase.FindAssets("t:Scene")) {
			string path = AssetDatabase.GUIDToAssetPath(guid);
			//ファイル名を抜き出し同一名称のものをロードする
			string fileName = System.IO.Path.GetFileName(path);
			if (fileName.Contains(sceneName)) {
				EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
				break;
			}
		}
#endif
	}
	//------------------------------------------------------------------------------------
	/// <summary>
	/// Editor上のシーンを解放
	/// </summary>
	/// <param name="sceneName">シーン名称
	void CloseEditorScene(string sceneName) {
#if UNITY_EDITOR
		// 指定したシーンがロードされているかを確認
		Scene scn = SceneManager.GetSceneByName(sceneName);
		if (!string.IsNullOrEmpty(scn.name) && scn.isLoaded)
			EditorSceneManager.CloseScene(scn, true);
#endif
	}
}

#if UNITY_EDITOR
/**
 * Inspector の表示を変更するエディタ拡張
 */
[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderInspector : Editor {
	public override void OnInspectorGUI() {
		serializedObject.Update();

		SceneLoader obj = target as SceneLoader;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("loadScene"), new GUIContent("Load Scene"));
		// シーン呼び出し
		if (GUILayout.Button("Open Scene")) {
			obj.LoadScene();
		}
		// シーン解放
		if (GUILayout.Button("Close Scene")) {
			obj.RemoveScene();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif