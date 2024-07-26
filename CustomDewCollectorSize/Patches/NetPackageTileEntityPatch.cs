using CustomDewCollectorSize;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(NetPackageTileEntity), nameof(NetPackageTileEntity.ProcessPackage))]
public class NetPackageTileEntityPatch
{

    static bool Prefix(NetPackageTileEntity __instance, World _world, GameManager _callbacks)
    {
        if (_world == null)
        {
            return false;
        }
        TileEntity tileEntity = __instance.bValidEntityId ? _world.GetTileEntity(__instance.teEntityId) : _world.GetTileEntity(__instance.clrIdx, __instance.teWorldPos);
        if (tileEntity == null)
        {
            return false;
        }
        TileEntityDewCollector dewCollector = tileEntity as TileEntityDewCollector;
        if (dewCollector == null)
        {
            return true;
        }

        // create a copy of a dew collector to protect server 
        TileEntityDewCollector dewCollectorCopy = new TileEntityDewCollector(dewCollector.chunk);
        dewCollectorCopy.SetHandle(__instance.handle);
        using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
        {
            PooledExpandableMemoryStream obj = __instance.ms;
            lock (obj)
            {
                pooledBinaryReader.SetBaseStream(__instance.ms);
                __instance.ms.Position = 0L;
                dewCollectorCopy.read(pooledBinaryReader, _world.IsRemote() ? TileEntity.StreamModeRead.FromServer : TileEntity.StreamModeRead.FromClient);
            }
        }
        
        Vector2i dewCollectorSize = dewCollectorCopy.GetContainerSize();
        if (dewCollectorSize.x == ModLoader.Columns && dewCollectorSize.y == ModLoader.Rows)
        {
            // only accept the net packet if the dew collector is the correct size
            dewCollector.SetHandle(__instance.handle);
            using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
            {
                PooledExpandableMemoryStream obj = __instance.ms;
                lock (obj)
                {
                    pooledBinaryReader.SetBaseStream(__instance.ms);
                    __instance.ms.Position = 0L;
                    dewCollector.read(pooledBinaryReader, _world.IsRemote() ? TileEntity.StreamModeRead.FromServer : TileEntity.StreamModeRead.FromClient);
                }
            }
            tileEntity.NotifyListeners();
            Vector3? entitiesInRangeOfWorldPos = new Vector3?(tileEntity.ToWorldCenterPos());
            if (entitiesInRangeOfWorldPos.Value == Vector3.zero)
            {
                entitiesInRangeOfWorldPos = null;
            }
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTileEntity>().Setup(tileEntity, TileEntity.StreamModeWrite.ToClient, __instance.handle), true, -1, -1, -1, entitiesInRangeOfWorldPos, 192);
            Log.Out($"updating server dew collector with data from client because it is correct size");
        }
        else
        {
            Log.Out($"ignoring request because client dew collector is not correct size");
        }

        return false;
    }
}