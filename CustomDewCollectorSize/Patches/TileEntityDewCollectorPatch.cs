using CustomDewCollectorSize;
using CustomDewCollectorSize.BlockUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;


[HarmonyPatch(typeof(TileEntityDewCollector), MethodType.Constructor, new Type[] { typeof(Chunk) })]
public class TileEntityDewCollectorSizePatch
{
    static void Postfix(ref Vector2i ___containerSize)
    {
        ___containerSize = new Vector2i(ModLoader.Columns, ModLoader.Rows);
    }
}

[HarmonyPatch(typeof(TileEntityDewCollector), MethodType.Constructor, new Type[] { typeof(TileEntityDewCollector) })]
public class TileEntityDewCollectorSizePatch2
{
    static void Postfix(TileEntityDewCollector _other, ref Vector2i ___containerSize)
    {
        ___containerSize = new Vector2i(ModLoader.Columns, ModLoader.Rows);
    }
}

[HarmonyPatch(typeof(NetPackageSetBlock), nameof(NetPackageSetBlock.ProcessPackage))]
public class TileEntityDewCollectorSizePatch3
{
    static bool Prefix(ref List<BlockChangeInfo> ___blockChanges)
    {
        for (int x = 0; x < ___blockChanges.Count; x++)
        {
            BlockChangeInfo blockChange = ___blockChanges[x];
            if (blockChange != null)
            {
                BlockValue dewCollectorBlockValue = BlockUtils.GetBlock("cntDewCollector").ToBlockValue();
                BlockValue blockChangeValue = blockChange.blockValue;
                if (blockChangeValue.Equals(dewCollectorBlockValue))
                {
                    // remove original block
                    ___blockChanges.RemoveAt(x);

                    // place block from server
                    GameManager.Instance.World.SetBlockRPC(blockChange.clrIdx, blockChange.pos, dewCollectorBlockValue);
                    Log.Out("Replaced player placed dew collector block with server placed dew collector");
                }
            }
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.dewCollectorOpened))]
public class TileEntityDewCollectorSizePatch4
{
    static void Postfix(TileEntityDewCollector _te, LocalPlayerUI _playerUI, int _entityIdThatOpenedIt)
    {
        Vector2i dewTile = _te.GetContainerSize();
        Log.Out($"Dew collector size at time of opening, columns: {dewTile.x} row: {dewTile.y}");
    }
}