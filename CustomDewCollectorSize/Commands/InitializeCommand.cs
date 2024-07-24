using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomDewCollectorSize.BlockUtils;
using CustomDewCollectorSize;

public class InitializeCommand : ConsoleCmdAbstract
{
    public override string[] getCommands()
    {
        return new string[] { "customdewcollector", "cdc" };
    }

    public override string getDescription()
    {
        return "initialize custom dew collector size by placing modified dew collector at player location.";
    }

    public override string getHelp()
    {
        return "cdc";
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        try
        {
            EntityPlayer player = GameManager.Instance.World.GetEntity(_senderInfo.RemoteClientInfo.entityId) as EntityPlayer;
            if (player != null)
            {
                Vector3i playerPosition = new Vector3i(player.position.x, player.position.y, player.position.z);
                Vector3i playerChunkPos = player.lastChunkPos;
                Log.Out($"placing block at position {playerPosition.x}, {playerPosition.y}, {playerPosition.z}");
                Coroutine watchChunksLoad = ThreadManager.StartCoroutine(WhenReady(playerChunkPos, playerPosition, player));
            }
        }
        catch (Exception e)
        {
            Log.Out($"Error retrieving player entity: {e.Message}");
        }
    }

    private IEnumerator WhenReady(Vector3i playerChunkPos, Vector3i playerPosition, EntityPlayer player)
    {
        Chunk chunk = null;
        while (chunk == null)
        {
            chunk = GameManager.Instance.World.GetChunkSync(playerChunkPos.x, playerChunkPos.z) as Chunk;
            yield return null;
        }
        try
        {
            try
            {
                Block dewCollectorBlock = BlockUtils.GetBlock("cntDewCollector");
                if ( dewCollectorBlock != null )
                {
                    GameManager.Instance.World.SetBlockRPC(chunk.ClrIdx, playerPosition, dewCollectorBlock.ToBlockValue());
                    SdtdConsole.Instance.Output("Dew collector placed successfully.");
                }
                else
                {
                    SdtdConsole.Instance.Output("Failed to place dew collector block.");
                }
            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output($"Error setting block to dew collector: {e.Message}");
            }
        }
        catch (Exception e)
        {
            SdtdConsole.Instance.Output($"Error retrieving player chunk: {e.Message}");
        }
    }
}