using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextSending : BaseMeshEffect {
	private const int OneSpriteVertex = 6;
	private bool _isEnd = false;
	private float _alpha = 0f;
	private int _charaCount = 0;
	private Text _text;
	private float _Rate = 0.1f;
	public Text Text
	{
		get { return _text ?? (_text = GetComponent<Text>()); }
	}
	public bool IsEnd()
	{
		return _isEnd;
	}
	public void SetRate(float f)
	{
		_Rate = f;
	}
	public void Initialize()
	{
		_charaCount = 0;
		_isEnd = false;
	}

	private List<UIVertex> _input = new List<UIVertex>();
	private List<UIVertex> _output = new List<UIVertex>();
	public override void ModifyMesh(VertexHelper vh)
	{
		var input = _input;
		_output.Clear();
		var output = _output;
		var text = Text;

		vh.GetUIVertexStream(input);
		var vertexTop = _charaCount * OneSpriteVertex;

		if(vertexTop >= input.Count)
		{
			_isEnd = true;
			return;
		}

		for (int i = 0; i < vertexTop; i++)
		{
			output.Add(input[i]);
		}

		for (int i = vertexTop; i < vertexTop+OneSpriteVertex; i++)
		{
			var uiVertex = input[i];
			uiVertex.color.a = (byte)(255f * _alpha);
			output.Add(uiVertex);
		}

		_alpha += _Rate;
		if(_alpha >= 1f)
		{
			_charaCount++;
			_alpha = 0f;
		}

		vh.Clear();
		vh.AddUIVertexTriangleStream(output);
	}
}
