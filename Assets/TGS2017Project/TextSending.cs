using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextSending : BaseMeshEffect {
	private const int OneSpriteVertex = 6;
	private bool _isEnd = false;
	private float _Time = 0f;
	private int _charaCount = 0;
	private Text _text;
	private float _Rate = 1f;
	private float _Delay = 1f;
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
	public void SetDelay(float f)
	{
		_Delay = f;
	}
	public void SetSkip(bool b)
	{
		if(b)_Time = _input.Count / OneSpriteVertex * _Delay;
	}
	public void Initialize()
	{
		_charaCount = 0;
		_isEnd = false;
		_Time = 0;
	}

	private List<UIVertex> _input = new List<UIVertex>();
	private List<UIVertex> _output = new List<UIVertex>();
	public override void ModifyMesh(VertexHelper vh)
	{
		_output.Clear();
		vh.GetUIVertexStream(_input);

		if(_Time >= _input.Count / OneSpriteVertex *_Delay + 1)
		{
			_isEnd = true;
			return;
		}

		for (int i = 0; i*OneSpriteVertex<_input.Count; i++)
		{
			for (int j = 0; j < OneSpriteVertex; j++)
			{
				_output.Add(Effect(_input[i * OneSpriteVertex + j], Mathf.Clamp01(_Time - i * _Delay)));
			}
		}

		_Time += _Rate * Time.deltaTime;

		vh.Clear();
		vh.AddUIVertexTriangleStream(_output);
	}

	private UIVertex Effect(UIVertex uiVertex, float t)
	{
		uiVertex.color.a = (byte)(255f * (t == 0 ? 0 : 1));
		return uiVertex;
	}
}
