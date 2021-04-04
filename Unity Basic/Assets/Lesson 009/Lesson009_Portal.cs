using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson009_Portal : MonoBehaviour
{
	public string destLevel;
	public bool loadAsync = false;
	public Lesson009_ScriptableObject scriptableObject;

    public void LoadLevel() {
        Debug.Log($"Load Scene {destLevel}");

		if (loadAsync) {
			StartCoroutine(LoadLevelAsync(destLevel));
			return;
		}

		UnityEngine.SceneManagement.SceneManager.LoadScene(destLevel);
	    Debug.Log($"Scene {destLevel} Loaded");
    }

    IEnumerator LoadLevelAsync(string level) {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(destLevel);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log($"Scene {destLevel} async Loaded");
    }

	private void OnDrawGizmos()
	{
		var box = GetComponent<BoxCollider>();
		if (box) {
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = scriptableObject ? scriptableObject.gizmosColor : Color.green;
			Gizmos.DrawCube(box.center, box.size);
		}
	}
}
