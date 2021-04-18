using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {
	//Generate a height map
	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
		//use the vlaues the generate a noise map
		float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

		//get the animation curve
		AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

		//calculate our min and max values
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		//Loop through and calculate our values from out data
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				//calculate from our height curve and using our height multiplier
				values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

				if (values[i, j] > maxValue) {
					maxValue = values[i, j];
				}
				if (values[i, j] < minValue) {
					minValue = values[i, j];
				}
			}
		}

		//return heightmap
		HeightMap heightmap = new HeightMap(values, minValue, maxValue);
		return heightmap;
	}

}
//struct to store a height map in
public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;

	public HeightMap(float[,] values, float minValue, float maxValue) {
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}
