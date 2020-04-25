//=======================================================================
//
// Atlas Frames Cache
// Description: Class uses to create sprites from atlas by sprite frame name
//
// Usage: CreateSpriteFromAtlas("atlas", "frame_1");
//
// Example: GameManager.Instance.AtlasFramesCache.CreateSpriteFromAtlas(UIConsts.ITEMS_ATLAS, "anvil.png");
// 
// Author: Mykhailo Zhovnach
// Company: ZagravaGames
// 2015/03
//=======================================================================


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class SpriteFrameData
{   
    public Vector4 border;
    //
    public string name;
    //
    public Vector2 pivot;
    //
    public Rect rect;
}

public class AtlasData
{
    public Texture2D texture;
    public Dictionary<string, SpriteFrameData> frames;

    public AtlasData()
    {
        texture = null;
        frames = new Dictionary<string, SpriteFrameData>();
    }
}

public class AtlasFramesCache
{
    private Dictionary<string, AtlasData> _atlases;

    public AtlasFramesCache()
    {
        _atlases = new Dictionary<string, AtlasData>();
    }

    private void LoadAtlas(string resPath, SpriteAlignment alignment = SpriteAlignment.Center)
    {        
        if (!_atlases.ContainsKey(resPath))
        {
            TextAsset xmlAsset = Resources.Load<TextAsset>(resPath);
            AtlasData atlas = new AtlasData();
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlAsset.text);
            XmlElement root = document.DocumentElement;
            if (root.Name == "TextureAtlas")
            {
                bool failed = false;
                atlas.texture = Resources.Load<Texture2D>(resPath);
                int textureHeight = atlas.texture.height;
                foreach (XmlNode childNode in root.ChildNodes)
                {
                    if (childNode.Name == "sprite")
                    {
                        try
                        {
                            int width = Convert.ToInt32(childNode.Attributes["w"].Value);
                            int height = Convert.ToInt32(childNode.Attributes["h"].Value);
                            int x = Convert.ToInt32(childNode.Attributes["x"].Value);
                            int y = textureHeight - (height + Convert.ToInt32(childNode.Attributes["y"].Value));

                            SpriteFrameData spriteMetaData = new SpriteFrameData
                            {                                
                                border = new Vector4(),
                                name = childNode.Attributes["n"].Value,
                                pivot = GetPivotValue(alignment),
                                rect = new Rect(x, y, width, height)
                            };

                            atlas.frames.Add(spriteMetaData.name, spriteMetaData);
                        }
                        catch (Exception exception)
                        {
                            failed = true;
                            Debug.LogException(exception);
                        }
                    }
                    else
                    {
                        Debug.Log("Child nodes should be named 'sprite' !");                        
                    }
                }

                if (!failed)
                {
                    _atlases.Add(resPath, atlas);
                }
            }
            else
            {
                Debug.Log("XML needs to have a 'TextureAtlas' root node!");
            }
        }
    }

    public AtlasData GetAtlas(string resPath)
    {
        if (!_atlases.ContainsKey(resPath))
        {
            LoadAtlas(resPath);
        }

        if (_atlases.ContainsKey(resPath))
        {
            return _atlases[resPath];
        }
        
        // error loading atlas
        Debug.Log("AtlasFramesCache: Error loading <" + resPath + "> atlas!");
        return null;
    }

    private static Vector2 GetPivotValue(SpriteAlignment alignment)
    {
        switch (alignment)
        {
            case SpriteAlignment.Center:
                return new Vector2(0.5f, 0.5f);
            case SpriteAlignment.TopLeft:
                return new Vector2(0.0f, 1f);
            case SpriteAlignment.TopCenter:
                return new Vector2(0.5f, 1f);
            case SpriteAlignment.TopRight:
                return new Vector2(1f, 1f);
            case SpriteAlignment.LeftCenter:
                return new Vector2(0.0f, 0.5f);
            case SpriteAlignment.RightCenter:
                return new Vector2(1f, 0.5f);
            case SpriteAlignment.BottomLeft:
                return new Vector2(0.0f, 0.0f);
            case SpriteAlignment.BottomCenter:
                return new Vector2(0.5f, 0.0f);
            case SpriteAlignment.BottomRight:
                return new Vector2(1f, 0.0f);
            default:
                return Vector2.zero;
        }
    }

    public void PreloadAtlasFrames(string resPath)
    {
        GetAtlas(resPath);
    }

    public Sprite CreateSpriteFromAtlas(string resPath, string spriteName)
    {
        AtlasData atlas = GetAtlas(resPath);
        if (atlas != null)
        {
            if (atlas.frames.ContainsKey(spriteName))
            {
                SpriteFrameData frame = atlas.frames[spriteName];
                return Sprite.Create(atlas.texture, frame.rect, frame.pivot);
            }
            else
            {
                Debug.Log("AtlasFramesCache: atlas <" + resPath + "> doesn't contains <" + spriteName + "> sprite!");                
            }
        }   
        // error loading atlas
        return null;
    }

    public bool ContainsFrame(string resPath, string spriteName)
    {
        AtlasData atlas = GetAtlas(resPath);
        if (atlas != null)
        {
            return atlas.frames.ContainsKey(spriteName);
        }
        return false;
    }
}
