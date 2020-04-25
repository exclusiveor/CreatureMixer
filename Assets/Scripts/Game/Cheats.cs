using UnityEngine;
using System.Collections;

public class Cheats
{
    public static bool IsCheatEnabled;
    public static string InputCheatcode;
    private static string UnlockCheatcode = "cheats";

    private static void CheckCheatcode(string ch)
    {
        InputCheatcode += ch;
        int currentLength = InputCheatcode.Length;
        if (currentLength > UnlockCheatcode.Length)
        {
            InputCheatcode = "" + InputCheatcode[currentLength - 1];
        } else
        {
            if (InputCheatcode[currentLength - 1] != UnlockCheatcode[currentLength - 1])
            {
                InputCheatcode = "" + InputCheatcode[currentLength - 1];
            }
        }
    }

    public static void CheckMatchCheats(GameBoard board)
    {
        if (IsCheatEnabled)
        {
            if (Input.GetKeyDown(KeyCode.R)) //Reshuffle Powerup
            {
                int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle];
                ++current;
                GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Reshuffle] = current;
                //
                EventData eventData = new EventData("OnPowerUpsResetNeededEvent");
                eventData.Data["isStart"] = true;
                GameManager.Instance.EventManager.CallOnPowerUpsResetNeededEvent(eventData);
            }

            if (Input.GetKeyDown(KeyCode.C)) // Chain Powerup
            {
                int current = GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain];
                ++current;
                GameManager.Instance.BoardData.PowerUps[GameData.PowerUpType.Chain] = current;
                //
                EventData eventData = new EventData("OnPowerUpsResetNeededEvent");
                eventData.Data["isStart"] = true;
                GameManager.Instance.EventManager.CallOnPowerUpsResetNeededEvent(eventData);
            }

            if (Input.GetKeyDown(KeyCode.L)) // Loose
            {
                board.OnLoose();
            }

            // time cheats
            if (Input.GetKey(KeyCode.T))
            {
                if (Time.timeScale == 0.1f)
                {
                    Time.timeScale = 1.0f;
                }
                else
                {
                    Time.timeScale = 0.1f;
                }
            }
        }
    }

    //public static void CheckMatchCheats(MatchBoard board)
    //   {

    //       if (IsCheatEnabled)
    //       {
    //           if (Input.touchCount == 3)
    //           {
    //               UnlockPowerUp(GameData.PowerUpType.Bomb);
    //               UnlockPowerUp(GameData.PowerUpType.Boots);
    //               UnlockPowerUp(GameData.PowerUpType.Lightning);
    //               UnlockPowerUp(GameData.PowerUpType.Morningstar);
    //               UnlockPowerUp(GameData.PowerUpType.Tornado);
    //               UnlockPowerUp(GameData.PowerUpType.Fire);
    //           }
    //           else
    //           if (Input.touchCount == 4)
    //           {
    //               int stars = 3;
    //               board.MatchBoardData.Scores.Stars = stars;
    //               board.MatchBoardData.BadgeStatistic.Badge = true;
    //               board.MatchBoardData.BadgeStatistic.GroundTilesCollected = board.MatchBoardData.BadgeStatistic.GroundTilesTotal;
    //               board.MatchBoardData.BadgeStatistic.TrasureChestsOpened = board.MatchBoardData.BadgeStatistic.TrasureChestsTotal;
    //               board.MatchBoardData.BadgeStatistic.ResourceVeinsDepleted = board.MatchBoardData.BadgeStatistic.ResourceVeinsTotal;
    //               board.MatchBoardData.BadgeStatistic.ItemsInStonesCollected = board.MatchBoardData.BadgeStatistic.ItemsInStonesTotal;
    //               board.MatchBoardData.BadgeStatistic.ClearedCrates = board.MatchBoardData.BadgeStatistic.TotalCrates;
    //               board.MatchBoardData.BadgeStatistic.GroundTilesCollected = board.MatchBoardData.BadgeStatistic.GroundTilesTotal;
    //               board.MatchBoardData.Scores.StarPoints -= 250;
    //               board.MatchBoardData.MoveCount = 0;
    //               board.BoardState = GameBoardState.GBS_BlowingAllBeforeFinish;
    //               board.SetBoardValue(BoardValueType.BVT_AllLevelStuffCleared);
    //           }
    //       }

    //       if (!IsCheatEnabled)
    //	{
    //		if (Input.GetKeyDown(KeyCode.Return))
    //		{
    //			if (InputCheatcode == UnlockCheatcode)
    //			{
    //				IsCheatEnabled = true;
    //			}
    //			InputCheatcode = "";
    //		} else
    //		if (Input.GetKeyDown(KeyCode.K))
    //		{
    //			CheckCheatcode("k");
    //		} else
    //		if (Input.GetKeyDown(KeyCode.I))
    //		{
    //			CheckCheatcode("i");
    //		} else
    //		if (Input.GetKeyDown(KeyCode.L))
    //		{
    //			CheckCheatcode("l");
    //		} else
    //		if (Input.GetKeyDown(KeyCode.A))
    //		{
    //			CheckCheatcode("a");
    //		} else
    //		if (Input.GetKeyDown(KeyCode.N))
    //		{
    //			CheckCheatcode("n");
    //		} else
    //		if (Input.anyKeyDown)
    //		{
    //			InputCheatcode = "";
    //		}
    //		return;
    //	}        

    //       Cell activeCell = board.CellOnMouse;
    //       if (activeCell != null)
    //       {
    //           if (!activeCell.Locked && activeCell.IsChip() && activeCell.Enabled)
    //           {
    //               if (Input.GetKeyDown(KeyCode.Keypad1))
    //               {
    //                   activeCell.ChipType = ChipType.Amphora;
    //                   activeCell.UpdateChipObject();                    
    //                   MatchAnimFabrik.InstantiateBoardEffect("SpecialChipIdleEffect", activeCell.ChipGameObject);
    //               }

    //               if (Input.GetKeyDown(KeyCode.Keypad2))
    //               {
    //                   activeCell.ChipType = ChipType.Bomb;
    //                   activeCell.UpdateChipObject();                    
    //                   MatchAnimFabrik.InstantiateBoardEffect("SpecialChipIdleEffect", activeCell.ChipGameObject);
    //               }
    //           }

    //           if (Input.GetKeyDown(KeyCode.S))
    //           {
    //               board.SaveCurrentLevel();
    //               GameManager.Instance.Settings.Save(true);
    //           }

    //           if (Input.GetKeyDown(KeyCode.K))
    //           {                
    //               activeCell.HiddenItem = GameData.ItemType.treasure_map;
    //               board.SetBoardValue(BoardValueType.BVT_CollectItem, 0, 0.0f, activeCell.BoardPos);                
    //           }

    //           if (Input.GetKeyDown(KeyCode.J))
    //           {
    //               activeCell.HiddenItem = GameData.ItemType.Insta_tornado;
    //               board.SetBoardValue(BoardValueType.BVT_CollectItem, 0, 0.0f, activeCell.BoardPos);
    //           }

    //           if (Input.GetKeyDown(KeyCode.O))
    //           {
    //               //activeCell.HiddenItem = GameData.ItemType.OilCan;
    //               activeCell.HiddenItem = MatchHelperTools.GetRandomResourcePot();
    //           }

    //           if (Input.GetKeyDown(KeyCode.P))
    //           {                
    //               board.MatchCore.NoMoreMove(board);
    //           }            			

    //		if (Input.GetKeyDown(KeyCode.L))
    //		{				
    //			activeCell.HiddenItem = GameManager.Instance.Settings.User.GenerateRandomArtefact();
    //			//activeCell.HiddenItem = GameManager.Instance.Settings.User.GenerateNextNeededArtefact();
    //		}

    //           //if (Input.GetKeyDown(KeyCode.Alpha1))
    //           //{
    //           //    board.SetBoardValue(BoardValueType.BVT_ReceiveResource, (int)ResourceType.Gold, 100, BoardPos.Zero);
    //           //}            

    //           if (Input.GetKeyDown(KeyCode.Alpha1))
    //           {
    //               activeCell.ChipType = ChipType.Food;
    //               activeCell.UpdateChipObject();
    //           }
    //           if (Input.GetKeyDown(KeyCode.Alpha2))
    //           {
    //               activeCell.ChipType = ChipType.Wood;
    //               activeCell.UpdateChipObject();
    //           }
    //           if (Input.GetKeyDown(KeyCode.Alpha3))
    //           {
    //               activeCell.ChipType = ChipType.Stone;
    //               activeCell.UpdateChipObject();
    //           }
    //           if (Input.GetKeyDown(KeyCode.Alpha4))
    //           {
    //               activeCell.ChipType = ChipType.BluePotion;
    //               activeCell.UpdateChipObject();
    //           }
    //           if (Input.GetKeyDown(KeyCode.Alpha5))
    //           {
    //               activeCell.ChipType = ChipType.Shield;
    //               activeCell.UpdateChipObject();
    //           }            

    //           if (Input.GetKeyDown(KeyCode.KeypadEnter))
    //           {
    //               board.MatchOnCheat();
    //           }
    //           if (Input.GetKeyDown(KeyCode.Space))
    //           {
    //               board.SetBoardValue(BoardValueType.BVT_OutOfMoves);
    //           }
    //           if (Input.GetKeyDown(KeyCode.PageUp))
    //           {
    //               Time.timeScale = 1.0f;
    //           }
    //           if (Input.GetKeyDown(KeyCode.PageDown))
    //           {
    //               Time.timeScale = 0.1f;
    //           }         

    //           if (Input.GetKeyDown(KeyCode.Equals))
    //           {
    //               GameManager.Instance.Player.MatchBoardData.CollectedResources[ResourceType.Gold] += 1000;                
    //           }

    //           if (Input.GetKeyDown(KeyCode.Q))
    //		{
    //			for (GameData.ItemType atype = GameData.ItemType.bandage; atype <= GameData.ItemType.machine_cog; ++atype)
    //			{
    //				GameManager.Instance.Settings.User.InventoryItems[atype] = 66; //UnityEngine.Random.Range(3, 5);
    //			}
    //		}

    //           if (Input.GetKeyDown(KeyCode.F1))
    //           {
    //               var powerup = PowerUpsBase.Create(GameData.PowerUpType.Tornado, BoardPos.Zero, board);
    //               board.PowerUpWorker.AddParalelAction(powerup);
    //           }

    //           if (Input.GetKeyDown(KeyCode.F2))
    //           {
    //               var powerup = PowerUpsBase.Create(GameData.PowerUpType.Fire, BoardPos.Zero, board);
    //               board.PowerUpWorker.AddParalelAction(powerup);
    //           }

    //           //r = clear all red tiles on a board
    //           if (Input.GetKeyDown(KeyCode.R))
    //           {
    //               Vector4 rect = board.ActiveRect;
    //               for (int y = (int)rect.y + 1; y < rect.w; y++)
    //               {
    //                   for (int x = (int)rect.x + 1; x < rect.z; x++)
    //                   {
    //                       Cell cell = board.GetCell(x, y);
    //                       if (cell != null && cell.TileType == TileType.TT_LevelTile
    //                           && cell.AreaIndex == board.MatchBoardData.ActiveArea)
    //                       {
    //                           board.AddParalelAction(new GetTileAction(board, cell));
    //                       }
    //                   }
    //               }

    //               board.AddParalelAction(new ExecuteAction(() => board.SetMustRefresh(), 1.0f));
    //           }

    //           //a = clear all non-red tiles on the board (and get the hidden items and obstacle unlockers)
    //           if (Input.GetKeyDown(KeyCode.A))
    //           {
    //               Vector4 rect = board.ActiveRect;
    //               for (int y = (int)rect.y + 1; y < rect.w; y++)
    //               {
    //                   for (int x = (int)rect.x + 1; x < rect.z; x++)
    //                   {
    //                       Cell cell = board.GetCell(x, y);
    //                       if (cell != null && cell.TileType > TileType.TT_Normal_2 &&
    //                           cell.TileType < TileType.TT_LevelTile
    //                           && cell.AreaIndex == board.MatchBoardData.ActiveArea)
    //                       {
    //                           board.AddParalelAction(new GetTileAction(board, cell));
    //                       }

    //                       if (cell != null && cell.CellType == CellType.CT_Fog)
    //                       {
    //                           board.AddParalelAction(new ExecuteAction(() => board.SetBoardValue(BoardValueType.BVT_ClearFogOnCell, 0, 0.0f, cell.BoardPos), 0.0f));                            
    //                       }

    //                       if (cell != null && (cell.CellType == CellType.CT_SpiderWeb || cell.CellType == CellType.CT_IronBars))
    //                       {
    //                           GameObject.Destroy(cell.BlockerGameObject);
    //                           cell.BlockerGameObject = null;
    //                           cell.Locked = false;
    //                           cell.Enabled = true;
    //                           cell.CellType = CellType.CT_Normal;
    //                       }   
    //                   }
    //               }

    //               board.AddParalelAction(new ExecuteAction(() => board.SetMustRefresh(), 1.0f));
    //           }

    //           //w = clear everything on else on a board(open chests, exploit resource veins, destroy stones etc.)
    //           if (Input.GetKeyDown(KeyCode.W))
    //           {
    //               Vector4 rect = board.ActiveRect;
    //               float tcDelay = 0.0f;
    //               for (int y = (int)rect.y + 1; y < rect.w; y++)
    //               {
    //                   for (int x = (int)rect.x + 1; x < rect.z; x++)
    //                   {
    //                       Cell cell = board.GetCell(x, y);
    //                       if (cell != null && cell.AreaIndex == board.MatchBoardData.ActiveArea)
    //                       {
    //                           if (cell.CellType == CellType.CT_TreasureChest)
    //                           {
    //                               board.AddParalelAction(new ExecuteAction(() => board.SetBoardValue(BoardValueType.BVT_UnlockTreasureChestByCheat, 0, 0.0f, cell.BoardPos), tcDelay));
    //                               tcDelay += 0.1f;
    //                           }                           
    //                           if (cell.TileType == TileType.TT_ItemRock)
    //                           {
    //                               board.AddParalelAction(new GetTileAction(board, cell));
    //                           }                            
    //                       }
    //                   }
    //               }

    //               board.AddParalelAction(new ExecuteAction(() => board.SetMustRefresh(), 1.0f));
    //           }

    //           //g = add 100 gold
    //           if (Input.GetKeyDown(KeyCode.G))
    //		{
    //			GameManager.Instance.Settings.User.AddGold(100);
    //		}

    //           if (Input.GetKeyDown(KeyCode.F9) || Input.GetKeyDown(KeyCode.F10) ||
    //               Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.F12))
    //           {
    //               int stars = 0;
    //               if (Input.GetKeyDown(KeyCode.F9)) stars = 0;
    //               if (Input.GetKeyDown(KeyCode.F10)) stars = 1;
    //               if (Input.GetKeyDown(KeyCode.F11)) stars = 2;
    //               if (Input.GetKeyDown(KeyCode.F12)) stars = 3;

    //               if (stars == 0)
    //               {
    //                   board.MatchBoardData.Scores.StarPoints = 0;
    //               }
    //               else
    //               {
    //                   //board.MatchBoardData.Scores.StarPoints = board.MatchBoardData.Scores.StarPointsNeeded[stars - 1];
    //				board.MatchBoardData.Scores.Stars = stars;
    //               }
    //               if (Input.GetKey(KeyCode.LeftControl))
    //               {
    //                   board.MatchBoardData.BadgeStatistic.Badge = true;
    //                   board.MatchBoardData.BadgeStatistic.GroundTilesCollected = board.MatchBoardData.BadgeStatistic.GroundTilesTotal;
    //                   board.MatchBoardData.BadgeStatistic.TrasureChestsOpened = board.MatchBoardData.BadgeStatistic.TrasureChestsTotal;
    //                   board.MatchBoardData.BadgeStatistic.ResourceVeinsDepleted = board.MatchBoardData.BadgeStatistic.ResourceVeinsTotal;
    //                   board.MatchBoardData.BadgeStatistic.ItemsInStonesCollected = board.MatchBoardData.BadgeStatistic.ItemsInStonesTotal;
    //                   board.MatchBoardData.BadgeStatistic.ClearedCrates = board.MatchBoardData.BadgeStatistic.TotalCrates;
    //                   board.MatchBoardData.BadgeStatistic.GroundTilesCollected = board.MatchBoardData.BadgeStatistic.GroundTilesTotal;
    //                   board.MatchBoardData.Scores.StarPoints -= 250;
    //               }
    //               board.MatchBoardData.MoveCount = 0;
    //               board.BoardState = GameBoardState.GBS_BlowingAllBeforeFinish;
    //               board.SetBoardValue(BoardValueType.BVT_AllLevelStuffCleared);
    //           }

    //           if (Input.GetKeyDown(KeyCode.M))
    //           {
    //               board.MatchBoardData.MoveCount = 1;
    //               EventData e = new EventData("");
    //               GameManager.Instance.EventManager.CallOnChangeMovesEvent(e);
    //           }
    //       }
    //   }

    //private static void UnlockPowerUp(GameData.PowerUpType type)
    //{
    //    if (GameManager.Instance.Settings.User.PowerUpsState[type] < 0)
    //    {
    //        GameManager.Instance.Settings.User.PowerUpsState[type] = 0;
    //        EventData e = new EventData("OnPowerUpChangedEvent");
    //        e.Data["ShowUnlockAnim"] = true;
    //        GameManager.Instance.EventManager.CallOnPowerUpChangedEvent(e);
    //        GameManager.Instance.Settings.User.PowerUpCounter[type] = 99;
    //    }
    //}
}
