using CustomDewCollectorSize;
using HarmonyLib;
using System;


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

[HarmonyPatch(typeof(NetPackageTileEntity), nameof(NetPackageTileEntity.Setup), new Type[] { typeof(TileEntity), typeof(TileEntity.StreamModeWrite), typeof(byte) })]
public class TileEntityDewCollectorSizePatch3
{
    static void Postfix(NetPackageTileEntity __instance, TileEntity _te, TileEntity.StreamModeWrite _eStreamMode, byte _handle)
    {
        // this is getting called every tic to update the dew collector
        TileEntityDewCollector dewCollector = _te as TileEntityDewCollector;
        if (dewCollector != null)
        {
            Vector2i expectedSize = new Vector2i(ModLoader.Columns, ModLoader.Rows);
            if (!dewCollector.GetContainerSize().Equals(expectedSize))
            {
                dewCollector.SetContainerSize(expectedSize);
                dewCollector.setModified();
            }
        }
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