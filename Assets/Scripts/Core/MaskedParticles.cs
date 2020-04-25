using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( ParticleSystem ) )]
public class MaskedParticles : MonoBehaviour 
{
    #region Properties
    public float  EmissionRate = 10f;
    #endregion
	
    float          _timeToEmission = 0f;
    ParticleSystem _particleSystem;

	private Vector2 _dXDy;
	
	public Sprite Mask;
	public GameObject ParentObject;
	public int GridSize = 5;   // px
	public bool QuickWay = true; // if true, than check only points in the middle of grids
	public float ParticlesPerGrid = 0.25f;
	public int ParticleEmissionCount = 1;

	public List<Vector2> FilledCells { get; set; }
	public float OpacityLimit = 0.5f; // if opacity >= OpacityLimit than this grid is "filled"
	private bool Playing = false;
	public bool Autoplay = false;
	public float PlayTime = -1;
	public bool DestroyOnComplete = false;

    float EmissionPeriod { get { return 1f / EmissionRate; } }

    void Awake () 
    {
        _particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule emission = _particleSystem.emission;
        emission.rate = new ParticleSystem.MinMaxCurve(0.0f);
        _timeToEmission = EmissionPeriod;
    }

    void Start () 
    {   
		if (ParentObject != null)
		{
			Init();
		}
		if (Autoplay)
		{
			Play();
		}
    }

	public void Play()
	{
		Playing = true;
		if (PlayTime > 0)
		{
			Invoke("Stop", PlayTime);
		}
	}

	public void Stop()
	{
		if (!Playing) { return; }
		Playing = false;
		if (DestroyOnComplete)
		{
			Invoke("DestroyEffect", _particleSystem.startLifetime);
		}
	}

	private void DestroyEffect()
	{
		GameObject.Destroy(gameObject);
	}

	public void Init()
	{
		Rect rect = Mask.rect;

		_dXDy = new Vector2(rect.width / 2.0f / 100.0f, rect.height / 2.0f / 100.0f);

		//Rect rect2 = Mask.textureRect;
		//float h = Mask.texture.height;
		//float w = Mask.texture.width;

		FilledCells = new List<Vector2>();
		int gWidth = (int)rect.width / GridSize;
		int gHeight = (int)rect.height / GridSize;
		Texture2D texture = Mask.texture;
		float pixelsInGrid = GridSize * GridSize;
		float divider = 100.0f / GridSize;
		if (!QuickWay)
		{
			for (int i = 0; i < gWidth; ++i)
			{
				for (int j = 0; j < gHeight; ++j)
				{
					// calculate average opacity
					float opacity = 0;
					for (int xx = 0; xx < GridSize; ++xx)
					{
						for (int yy = 0; yy < GridSize; ++yy)
						{
							opacity += texture.GetPixel((int)rect.x + i * GridSize + xx, (int)rect.y + j * GridSize + yy).a;
						}
					}
					opacity /= pixelsInGrid;
					if (opacity >= OpacityLimit)
					{
						FilledCells.Add(new Vector2(i / divider, j / divider));
						//Debug.Log (i.ToString() + " : " + j.ToString() + "    " + FilledCells[FilledCells.Count - 1].ToString());
					}
				}
			}
		} else
		{
			int halfGridSize = GridSize / 2;
			for (int i = 0; i < gWidth; ++i)
			{
				for (int j = 0; j < gHeight; ++j)
				{
					// get opacity in center of grid
					float opacity = texture.GetPixel((int)rect.x + i * GridSize + halfGridSize, (int)rect.y + j * GridSize + halfGridSize).a;
					if (opacity >= OpacityLimit)
					{
						FilledCells.Add(new Vector2(i / divider, j / divider));
						//Debug.Log (i.ToString() + " : " + j.ToString() + "    " + FilledCells[FilledCells.Count - 1].ToString());
					}
				}
			}
		}
	}

    void Update () 
    {
        //_shapeIndex += EmissionShapeSwitchSpeed * Time.deltaTime;
        //if( _shapeIndex > EmissionShapes.Length-1 ) _shapeIndex -= (EmissionShapes.Length-1);

		if (!Playing)
		{
			return;
		}

        _timeToEmission -= Time.deltaTime;
        if( _timeToEmission <= 0f )
        {
            _timeToEmission = EmissionPeriod - _timeToEmission;

			int particlesCount = (int)(ParticlesPerGrid * FilledCells.Count);
			if (particlesCount <= 0)
			{
				particlesCount = 1;
			}
			for (int c = 0; c < particlesCount; ++c)
			{
				// TODO rotations! convert points with rotations! use UV matrix?
				Vector2 randCellPos = FilledCells[UnityEngine.Random.Range(0, FilledCells.Count)];
				float x = (UnityEngine.Random.Range(randCellPos.x, randCellPos.x + GridSize / 100.0f) - _dXDy.x) * ParentObject.transform.localScale.x;
				float y = (UnityEngine.Random.Range(randCellPos.y, randCellPos.y + GridSize / 100.0f) - _dXDy.y) * ParentObject.transform.localScale.y;
				
				Vector3 pos = new Vector3(x, y, 0);
				Vector3 veloc = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * _particleSystem.startSpeed, UnityEngine.Random.Range(-1.0f, 1.0f) * _particleSystem.startSpeed, 0);				
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                emitParams.velocity = veloc;
                emitParams.position = pos;
                emitParams.startSize = _particleSystem.startSize;
                emitParams.startLifetime = _particleSystem.startLifetime;
                emitParams.startColor = _particleSystem.startColor;
				_particleSystem.Emit (emitParams, ParticleEmissionCount);
			}
        }
    }
}