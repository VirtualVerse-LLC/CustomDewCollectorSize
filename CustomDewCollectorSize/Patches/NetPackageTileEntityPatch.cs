using CustomDewCollectorSize;
using HarmonyLib;
using System;

[HarmonyPatch(typeof(NetPackageTileEntity), nameof(NetPackageTileEntity.ProcessPackage))]
public class NetPackageTileEntityPatch
{

    static bool Prefix(NetPackageTileEntity __instance, World _world, GameManager _callbacks)
    {
        if (_world == null)
        {
            return true;
        }
        TileEntity tileEntity = __instance.bValidEntityId ? _world.GetTileEntity(__instance.teEntityId) : _world.GetTileEntity(__instance.clrIdx, __instance.teWorldPos);
        if (tileEntity == null)
        {
            return true;
        }
        TileEntityDewCollector dewCollector = tileEntity as TileEntityDewCollector;
        if (dewCollector == null)
        {
            return true;
        }
        dewCollector.SetHandle(__instance.handle);
        using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(_bReset: false))
        {
            lock (__instance.ms)
            {
                pooledBinaryReader.SetBaseStream(__instance.ms);
                __instance.ms.Position = 0L;
                dewCollector.read(pooledBinaryReader, _world.IsRemote() ? TileEntity.StreamModeRead.FromServer : TileEntity.StreamModeRead.FromClient);
            }
        }

        Vector2i dewCollectorSize = dewCollector.GetContainerSize();
        if (dewCollectorSize.x != ModLoader.Columns || dewCollectorSize.y != ModLoader.Rows)
        {
            dewCollector.SetContainerSize(new Vector2i(ModLoader.Columns, ModLoader.Rows));
            if (dewCollector.items.Length != ModLoader.Columns * ModLoader.Rows)
            {
                dewCollector.items = ItemStack.CreateArray(ModLoader.Columns * ModLoader.Rows);
            }
        }
        if (dewCollector.fillValues.Length != (ModLoader.Columns * ModLoader.Rows))
        {
            dewCollector.fillValues = new float[ModLoader.Columns * ModLoader.Rows];
        }
        dewCollector.setModified();

        return false;
    }
}

[HarmonyPatch(typeof(NetPackageTileEntity), nameof(NetPackageTileEntity.Setup), new Type[] {typeof(TileEntity), typeof(TileEntity.StreamModeWrite), typeof(byte) })]
public class NetPackageTileEntityPatch2
{

    static bool Prefix(NetPackageTileEntity __instance, ref TileEntity _te, TileEntity.StreamModeWrite _eStreamMode, byte _handle)
    {
        if (_te == null)
        {
            return true;
        }
        TileEntityDewCollector dewCollector = _te as TileEntityDewCollector;
        if (dewCollector == null)
        {
            return true;
        }
        Vector2i dewCollectorSize = dewCollector.GetContainerSize();
        if (dewCollectorSize.x != ModLoader.Columns || dewCollectorSize.y != ModLoader.Rows)
        {
            dewCollector.SetContainerSize(new Vector2i(ModLoader.Columns, ModLoader.Rows));
            if (dewCollector.items.Length != ModLoader.Columns * ModLoader.Rows)
            {
                dewCollector.items = ItemStack.CreateArray(ModLoader.Columns * ModLoader.Rows);
                dewCollector.fillValues = new float[ModLoader.Columns * ModLoader.Rows];
            }
            dewCollector.setModified();
            _te = dewCollector;
        }
        if (dewCollector.fillValues.Length != (ModLoader.Columns * ModLoader.Rows))
        {
            dewCollector.fillValues = new float[ModLoader.Columns * ModLoader.Rows];
        }
        return true;
    }
}