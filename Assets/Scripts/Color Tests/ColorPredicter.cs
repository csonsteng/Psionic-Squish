using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

public static class ColorPredicter 
{
	private static int[] layerSizes;
	private static NeuralNetwork activeNetwork;
	private static List<string> colorOptions;
	private static List<ColorData> colorData;



	public static void Initialize(List<string> colors) {
		Debug.Log($"Initializing with {colors.Count} colors");
		layerSizes = new int[]{3, 7, 7, colors.Count};
		colorOptions = colors;

		//activeNetwork = NewActiveNetwork();
	}

	private static NeuralNetwork NewActiveNetwork() {
		var network = new NeuralNetwork();
		return network;
	}

	public static async void Teach(List<ColorData> data, int poolSize = 100) {
		var startTime = System.DateTime.Now;
		colorData = data;
		List<NeuralNetwork> firstGenerationCandidates = new List<NeuralNetwork>();
		for(var i = 0; i < poolSize; i++) {
			firstGenerationCandidates.Add(new NeuralNetwork());
		}
		//NeuralNetwork bestCandidate = firstGenerationCandidates[0];
		List<UniTask<NeuralNetwork>> candidates = new List<UniTask<NeuralNetwork>>();
		foreach (var candidate in firstGenerationCandidates) {
			candidates.Add(Propogate(candidate, poolSize));
			//candidate.Test();
			//if (candidate.accuracy >= bestCandidate.accuracy) {
			//	bestCandidate = candidate;
			//}
		}

		//Debug.Log($"Generation 1 Accuracy: {bestCandidate.accuracy * 100:0}%");
		//bestCandidate.Teach();
		//int maxIterations = 100;
		//activeNetwork = bestCandidate;
		//var iteration = 0;
		//while(activeNetwork.accuracy < 1f && iteration < maxIterations) {
		//	activeNetwork = new NeuralNetwork(activeNetwork);
		//	activeNetwork.Teach();
		//	Debug.Log($"Generation {activeNetwork.generation} Accuracy: {activeNetwork.accuracy * 100:0}%");

		//	iteration++;
		//}

		var finalCandidates = await UniTask.WhenAll(candidates);
		float accuracy = 0;
		foreach(var candidate in finalCandidates) {
			if(candidate.accuracy >= accuracy) {
				accuracy = candidate.accuracy;
				activeNetwork = candidate;
			}
		}
		var endTime = System.DateTime.Now;
		Debug.Log($"Finished with an accuracy of {activeNetwork.accuracy*100:0}%");
		Debug.Log($"Took {endTime.Subtract(startTime).TotalSeconds:0.00} seconds");
	}

	private static async UniTask<NeuralNetwork> Propogate(NeuralNetwork network, int iterations) {
		network.Teach();
		int maxIterations = iterations;
		var iteration = 0;
		while (network.accuracy < 1f && iteration < maxIterations) {
			network = new NeuralNetwork(network);
			network.Teach();
			iteration++;
		}
		await UniTask.CompletedTask;
		return network;
	}

	private static void Tweak() {

		
	}





	private class NeuralNetwork {
		public List<Matrix> weights;
		public List<Matrix> biases;
		public List<Matrix> weightGradients;
		public List<Matrix> biasGradients;
		public float accuracy;
		public int generation = 1;
		public float averageCost;

		public NeuralNetwork parent;
		public NeuralNetwork heir;

		public List<NeuralNetwork> children = new List<NeuralNetwork>();

		private int trainingData = 0;

		public NeuralNetwork() {
			weights = new List<Matrix>();
			biases = new List<Matrix>();
			weightGradients = new List<Matrix>();
			biasGradients = new List<Matrix>();

			for (var i = 1; i < layerSizes.Length; i++) {
				var weight = new float[layerSizes[i], layerSizes[i - 1]];
				var bias = new float[layerSizes[i]];
				for (var p = 0; p < layerSizes[i]; p++) {
					for (var q = 0; q < layerSizes[i - 1]; q++) {
						weight[p, q] = Random.Range(-1.5f, 1.5f); // should be standard distribution
					}
					bias[p] = 0;
				}
				weights.Add(new Matrix(weight));
				weightGradients.Add(new Matrix(new float[layerSizes[i], layerSizes[i - 1]]));
				biases.Add(new Matrix(bias));
				biasGradients.Add(new Matrix(new float[layerSizes[i]]));
			}
			ClearGradients();
		}

		public NeuralNetwork(NeuralNetwork parent, float alterChance, float alterRange) {
			this.parent = parent;
			weights = new List<Matrix>();
			biases = new List<Matrix>();
			weightGradients = new List<Matrix>();
			biasGradients = new List<Matrix>();

			foreach (var weight in parent.weights) {
				weights.Add(weight.Copy());
				weightGradients.Add(weight.Copy());
			}
			foreach (var bias in parent.biases) {
				biases.Add(bias.Copy());
				biasGradients.Add(bias.Copy());
			}
			ClearGradients();

			//scale alterations based on generation. Higher generations should be tweaked less
			Mutate(alterChance, alterRange);
			generation = parent.generation + 1;
		}

		public NeuralNetwork(NeuralNetwork parent) {
			this.parent = parent;
			weights = new List<Matrix>();
			biases = new List<Matrix>();
			weightGradients = new List<Matrix>();
			biasGradients = new List<Matrix>();

			foreach (var weight in parent.weights) {
				weights.Add(weight.Copy());
			}
			foreach (var bias in parent.biases) {
				biases.Add(bias.Copy());
			}

			for (var i = 1; i < layerSizes.Length; i++) {
				weightGradients.Add(new Matrix(new float[layerSizes[i], layerSizes[i - 1]]));
				biasGradients.Add(new Matrix(new float[layerSizes[i]]));
			}

				Improve();
			generation = parent.generation + 1;
		}

		private void ClearGradients() {
			var gradients = new List<Matrix>();
			gradients.AddRange(biasGradients);
			gradients.AddRange(weightGradients);
			foreach(var gradient in gradients) {
				//gradient.Clear();
			}
		}

		public void Improve() {
			for (var i = 0; i < weights.Count; i++) {
				foreach (var element in weights[i].Elements()) {
					element.Set(parent.weightGradients[i].values[element.j, element.i] + element.Get()*parent.averageCost);
				}
			}
			for (var i = 0; i < biases.Count; i++) {
				foreach (var element in biases[i].Elements()) {
					element.Set(parent.biasGradients[i].values[element.j, element.i] + element.Get() * parent.averageCost);
				}
			}
		}

		public void Mutate(float alterChance, float alterRange) {
			foreach(var weight in weights) {
				weight.Mutate(alterChance, alterRange);
			}
			foreach (var bias in biases) {
				bias.Mutate(alterChance, alterRange);
			}
		}

		public Matrix Predict(Vector3 input) {
			if (weights.Count != biases.Count) {
				throw new System.Exception("Mismatch between number of weight matrices and bias matrices");
			}
			var matrix = new Matrix(new float[] { input.x, input.y, input.z });
			for (var i = 0; i < weights.Count; i++) {
				matrix = matrix.Times(weights[i], biases[i]);
			}
			return matrix;
		}

		public Matrix Learn(Vector3 input, int correctAnswerIndex) {
			var matrix = Predict(input);
			float cost = matrix.Cost(correctAnswerIndex);
			averageCost = (averageCost * trainingData + cost) / (trainingData + 1);
			float stepSize = 0.1f;
			for (var i = 0; i < weights.Count; i++) {
				foreach (var element in weights[i].Elements()) {
					var originalValue = element.Get();
					element.Set(originalValue * (1 + stepSize));
					var alteredCost = Predict(input).Cost(correctAnswerIndex);
					var deltaCost = alteredCost - cost;
					var derivitave = deltaCost / stepSize;
					var currentGradientValue = weightGradients[i].values[element.j, element.i];
					var totalValue = currentGradientValue * trainingData;
					var newValue = (totalValue + derivitave) / (trainingData + 1);
					weightGradients[i].values[element.j, element.i] = newValue;
					element.Set(originalValue);
				}
			}
			for (var i = 0; i < biases.Count; i++) {
				foreach (var element in biases[i].Elements()) {
					var originalValue = element.Get();
					element.Set(originalValue * (1 + stepSize));
					var alteredCost = Predict(input).Cost(correctAnswerIndex);
					var deltaCost = alteredCost - cost;
					var derivitave = deltaCost / stepSize;
					var currentGradientValue = biasGradients[i].values[element.j, element.i];
					var totalValue = currentGradientValue * trainingData;
					var newValue = (totalValue + derivitave) / (trainingData + 1);
					biasGradients[i].values[element.j, element.i] = newValue;
					element.Set(originalValue);
				}
			}
			trainingData++;
			return matrix;
		}

		public void Teach() {
			int correctGuesses = 0;
			foreach (var color in colorData) {
				var prediction = Learn(color.AsVector3(), colorOptions.IndexOf(color.color));
				if (colorOptions[prediction.MaxIndex()] == color.color) {
					correctGuesses++;
				}
			}
			accuracy = (float)correctGuesses / colorData.Count;
		}



		public NeuralNetwork BestCandidate() {
			NeuralNetwork bestCandidate = this;
			float highestAccuracy = accuracy;
			foreach (var child in children) {
				if(child.accuracy > highestAccuracy) {
					bestCandidate = child;
				}
			}
			if(bestCandidate != this) {
				heir = bestCandidate;

				Debug.Log($"Generation {heir.generation} Accuracy: {heir.accuracy * 100:0}%");
			}
			return bestCandidate;
		}

		public void Birth(int numberOfChildren) {
			for(var i = 0; i < numberOfChildren; i++) {
				var child = new NeuralNetwork(this);
				child.Test();
				children.Add(child);
			}
		}

		public void Test() {
			int correctGuesses = 0;
			foreach (var color in colorData) {
				var prediction = Predict(color.AsVector3());
				if (colorOptions[prediction.MaxIndex()] == color.color) {
					correctGuesses++;
				}
			}
			accuracy = (float)correctGuesses / colorData.Count;
		}
	}
}

[System.Serializable]
public class Matrix {
	[SerializeField]
	public float[,] values;

	public Matrix(float[,] values) {
		this.values = values;
	}

	public Matrix(float[] values) {
		this.values = new float[values.Length, 1];
		for (var i = 0; i < values.Length; i++) {
			this.values[i, 0] = values[i];
		}
	}

	public Matrix Copy() {
		return new Matrix(values);
	}

	public string Dimensions => $"{Rows} X {Columns}";

	public int Rows => values.GetLength(1);

	public int Columns => values.GetLength(0);

	public int MaxIndex() {
		if (Rows != 1) {
			throw new System.Exception($"Max Index can only be found for column vectors ({Dimensions})");
		}
		float max = values[0, 0];
		int maxIndex = 0;
		for (var i = 0; i < Columns; i++) {
			if (values[i, 0] > max) {
				max = values[i, 0];
				maxIndex = i;
			}
		}
		return maxIndex;
	}

	public float Cost(int correctAnswerIndex) {
		if (Rows != 1) {
			throw new System.Exception($"Cost can only be found for column vectors ({Dimensions})");
		}
		float[,] costMatrix = new float[values.Length, 1];
		float cost = 0;
		for (var i = 0; i < Columns; i++) {
			float stepCost;
			if (i == correctAnswerIndex) {
				stepCost = (values[i, 0] - 1) * (values[i, 0] - 1);
			}
			else {
				stepCost = values[i, 0] * values[i, 0];
			}
			costMatrix[i, 0] = stepCost;
			cost += stepCost;
		}
		return cost;
	}

	public Matrix Times(Matrix other, Matrix linearBias = null) {
		if (other.Rows != Columns) {
			throw new System.Exception($"Invalid Matrix Multiplication {Dimensions} times {other.Dimensions}");
		}
		if (linearBias == null) {
			linearBias = new Matrix(new float[other.Columns]);
			for (var i = 0; i < other.Columns; i++) {
				linearBias.values[i, 0] = 0;
			}
		}

		if (linearBias.Columns != other.Columns) {
			throw new System.Exception($"Linear Bias Size mismatch {other.Columns} plus {linearBias.Dimensions}");
		}

		var result = new float[other.Columns, Rows];

		for (var i = 0; i < other.Columns; i++) {
			for (var j = 0; j < Rows; j++) {
				float a = 0;
				for (var q = 0; q < Columns; q++) {
					a += values[q, j] * other.values[i, q] + linearBias.values[i, 0];
				}
				result[i, j] = F(a);
			}
		}

		return new Matrix(result);
	}

	private float F(float x) {
		return 1 / (1 + Mathf.Exp(-x));
	}

	public void Print() {
		string matrix = "";
		for (var i = 0; i < Rows; i++) {
			var line = "[";
			for (var j = 0; j < Columns; j++) {
				line += values[j, i] + ", ";
			}
			line += "]\n";
			matrix += line;
		}
		Debug.Log(matrix);
	}

	public void Clear() {
		foreach(var element in Elements()) {
			element.Set(0f);
		}
	}

	public void Mutate(float alterChance, float alterRange) {
		for (var i = 0; i < Rows; i++) {
			for (var j = 0; j < Columns; j++) {
				var rollChance = Random.Range(0, 1);
				if(rollChance >= alterChance) {
					continue;
				}
				var rollChange = Random.Range(-alterRange, alterRange);
				values[j, i] += rollChange;

			}

		}
	}

	public IEnumerable<Element> Elements() {
		for (var i = 0; i < Rows; i++) {
			for (var j = 0; j < Columns; j++) {
				yield return new Element(j, i, this);

			}

		}
	}

	public struct Element {
		public readonly int i;
		public readonly int j;
		readonly Matrix matrix;

		public Element(int column, int row, Matrix matrix) {
			i = row;
			j = column;
			this.matrix = matrix;
		}

		public int Index() {
			return j * i*matrix.Rows;
		}
		public void Set(float newValue) {
			matrix.values[j, i] = newValue;
		}

		public float Get() {
			return matrix.values[j, i];
		}

		
	}
}
