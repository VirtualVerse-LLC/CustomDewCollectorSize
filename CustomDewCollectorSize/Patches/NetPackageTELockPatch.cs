using CustomDewCollectorSize;
using HarmonyLib;
using static NetPackageTELock;

[HarmonyPatch(typeof(NetPackageTELock), nameof(NetPackageTELock.ProcessPackage))]
public class NetPackageTELockPatch
{

    static bool Prefix(NetPackageTELock __instance, World _world, GameManager _callbacks)
    {
        if (_world == null)
        {
            return true;
        }
        TileEntity tileEntity = _world.GetTileEntity(__instance.clrIdx, new Vector3i(__instance.posX, __instance.posY, __instance.posZ));
        if (tileEntity == null)
        {
            return true;
        }
        TileEntityDewCollector dewCollector = tileEntity as TileEntityDewCollector;
        if (dewCollector == null)
        {
            return true;
        }
        Vector2i dewCollectorSize = dewCollector.GetContainerSize();
        if (__instance.type == TELockType.LockServer)
        {
            if (dewCollectorSize.x != ModLoader.Columns || dewCollectorSize.y != ModLoader.Rows)
            {
                dewCollector.SetContainerSize(new Vector2i(ModLoader.Columns, ModLoader.Rows));
                if (dewCollector.items.Length != ModLoader.Columns * ModLoader.Rows)
                {
                    dewCollector.items = ItemStack.CreateArray(ModLoader.Columns * ModLoader.Rows);
                    dewCollector.fillValues = new float[ModLoader.Columns * ModLoader.Rows];
                }
                dewCollector.setModified();
            }
            if (dewCollector.fillValues.Length != (ModLoader.Columns * ModLoader.Rows))
            {
                dewCollector.fillValues = new float[ModLoader.Columns * ModLoader.Rows];
            }
        }
        return true;
    }
}