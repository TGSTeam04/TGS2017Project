using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaineBufferRender : MonoBehaviour {
	public RenderTexture source;
	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(source, dest);
	}
}
