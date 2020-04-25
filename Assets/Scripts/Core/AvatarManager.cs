using System;
using System.Collections.Generic;
using UnityEngine;
	
	public class AvatarManager
	{
		private static List<Texture2D> avatarArr;

		public AvatarManager ()
		{
			
		}

		public static void addAvatarToArray(string userFBName, Texture2D newAvatarTexture2D)
		{
			if (avatarArr == null)
			{
				avatarArr = new List<Texture2D>();
			}
			
			newAvatarTexture2D.name = userFBName;

			avatarArr.Add(newAvatarTexture2D);
		}
		
		public static Texture2D getAvatarByName(string userFBName)
		{
			if (avatarArr==null)
			{
				return null;
			}
			
			for (int i = 0; i < avatarArr.Count; i++)
			{
				if (avatarArr[i].name == userFBName)
				{
					return avatarArr[i];
				}
			}

			return null;
		}

	}