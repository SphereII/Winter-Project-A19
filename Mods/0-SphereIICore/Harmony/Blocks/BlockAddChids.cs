using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
class SphereII_Block_Workarounds
{
    // Removes the WRN about block position WRN No chunk for position -2368, 23, -696, can not add childs to pos -2369, 22, -696! Block cntCollapsedChemistryStation
    [HarmonyPatch(typeof(Block.MultiBlockArray))]
    [HarmonyPatch("AddChilds")]
    public class SphereII_Block_AddChilds
    {
        // Loops around the instructions and removes the return condition.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Grab all the instructions
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_0)
                {
                    codes[i].opcode = OpCodes.Ldc_I4_1;
                    break;

                }
            }

            return codes.AsEnumerable();
        }
    }



}

