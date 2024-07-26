using CustomDewCollectorSize;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(NetPackageSetBlock), nameof(NetPackageSetBlock.ProcessPackage))]
public class NetPackageSetBlockPatch
{

    static void Postfix(NetPackageSetBlock __instance, World _world, GameManager _callbacks)
    {

        for (int x = 0; x < __instance.blockChanges.Count; x++)
        {
            BlockChangeInfo blockChangeInfo = __instance.blockChanges[x];
            if (blockChangeInfo.blockValue.Block is BlockDewCollector)
            {
                if (_world == null)
                {
                    return;
                }
                TileEntity tileEntity =  _world.GetTileEntity(blockChangeInfo.clrIdx, blockChangeInfo.pos);
                if (tileEntity == null)
                {
                    return;
                }
                TileEntityDewCollector dewCollector = tileEntity as TileEntityDewCollector;
                if (dewCollector == null)
                {
                    return;
                }

                // If the server does not have the correct size, update the clients
                Vector2i dewColletorContainerSize = dewCollector.GetContainerSize();
                if (dewColletorContainerSize.x != ModLoader.Columns || dewColletorContainerSize.y != ModLoader.Rows)
                {
                    Log.Out($"dew collector placed with size, columns: {dewColletorContainerSize.x} rows: {dewColletorContainerSize.y}, updating clients");
                    // Send updated tile entity to clients
                    dewCollector.SetContainerSize(new Vector2i(ModLoader.Columns, ModLoader.Rows), true);
                    Vector3? entitiesInRangeOfWorldPos = new Vector3?(tileEntity.ToWorldCenterPos());
                    if (entitiesInRangeOfWorldPos.Value == Vector3.zero)
                    {
                        entitiesInRangeOfWorldPos = null;
                    }
                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTileEntity>().Setup(dewCollector, TileEntity.StreamModeWrite.ToClient, byte.MaxValue), true, -1, -1, -1, entitiesInRangeOfWorldPos, 192);
                }
            }
        }
    }
}