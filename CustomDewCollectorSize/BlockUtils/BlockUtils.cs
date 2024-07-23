using System;

namespace CustomDewCollectorSize.BlockUtils
{
    public static class BlockUtils
    {
        public static Block GetBlock(string blockName)
        {
            for (int i = 0; i < Block.list.Length; i++)
            {
                Block block = Block.list[i];
                if (block != null && block.GetBlockName().Equals(blockName, StringComparison.OrdinalIgnoreCase))
                {
                    return block;
                }
            }
            return null;
        }
    }
}
