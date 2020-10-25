using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PaintingGenerator : MonoBehaviour {
	public Material canvas;
	public Texture painting;
	public Material outputMaterial;

	[Button]
	public void AdjustHeight() {
		var ratio = (float)painting.width / painting.height;
		transform.localScale = 
			new Vector3(transform.localScale.x, transform.localScale.x /ratio, transform.localScale.z);
	}
	
	[Button]
	public void AdjustOutputMaterial() {
		GetComponent<MeshRenderer>().material = outputMaterial;
		AdjustOutputMaterialCanvas();
		outputMaterial.mainTexture = painting;
	}

	void AdjustOutputMaterialCanvas() {
		outputMaterial.EnableKeyword("_DETAIL_MULX2");
		outputMaterial.SetTexture("_DetailAlbedoMap", canvas.mainTexture);
		outputMaterial.SetTexture("_DetailNormalMap", canvas.GetTexture("_BumpMap"));
		outputMaterial.SetTextureScale("_DetailAlbedoMap",GetCanvasScaleForScaledCube(transform.localScale));
	}

	Vector2 GetCanvasScaleForScaledCube(Vector3 size) {
		float repetitionsPerMeter = 5;
		var xScale = size.x * repetitionsPerMeter;
		var yScale = size.y * repetitionsPerMeter;
		return new Vector2(xScale,yScale);
	}

	Vector2 GetTextureDimensions(Texture tex) {
		return new Vector2(tex.width, tex.height);
	}

	float GetTextureRatio(Texture tex) {
		return (float) tex.width / (float) tex.height;
	}
}
