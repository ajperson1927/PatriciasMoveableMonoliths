using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PAT.MoveableMonoliths;

[StaticConstructorOnStartup]
public static class Building_VoidMonolith_Patch
{
    static Building_VoidMonolith_Patch()
    {
        Log.Message("Starting");
        
        var harmony = new Harmony("com.PAT.MoveableMonoliths");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Building_VoidMonolith), "Tick")]
public static class Tick_Patch
{
    static void Prefix()
    {
        // Log.Message("Tick");
    }
    private static readonly MethodInfo PrettyPleaseLogMethod = AccessTools.Method(typeof(Tick_Patch), nameof(Tick_Patch.PrettyPleaseLog));
    private static readonly MethodInfo IsHashTickIntervalMethod = AccessTools.Method(typeof(Gen), "IsHashIntervalTick", [typeof(Thing), typeof(int)]);
    private static readonly MethodInfo LogMessageMethod = AccessTools.Method(typeof(Log), nameof(Message), [typeof(String)]);
    private static readonly MethodInfo GetLevelMethod = AccessTools.Method(typeof(GameComponent_Anomaly), "get_Level");
    private static readonly MethodInfo IsReservedMethod = AccessTools.Method(typeof(ReservationManager), nameof(ReservationManager.IsReserved));
    
    private static readonly FieldInfo Level0HintEffecterField = AccessTools.Field(typeof(Building_VoidMonolith), "level0HintEffecter");

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);


        // yield return new CodeInstruction(OpCodes.Ldstr, "Start of tick");
        // yield return new CodeInstruction(OpCodes.Call, LogMessageMethod);
        // foreach (var codeInstruction in LogMessage("Start of tick"))
        //     yield return codeInstruction;
        
        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction instruction = codes[i];
            CodeInstruction previous = null;
            CodeInstruction next = null;
            
            if (i > 0) previous = codes[i - 1];
            
            if (i < codes.Count - 1) next = codes[i + 1];
        
            // Check if IsHashIntervalTick returned true
            if (instruction.opcode == OpCodes.Brfalse_S && previous != null)
            {
                
                if (previous.opcode == OpCodes.Call && 
                    previous.operand is MethodInfo method &&
                    method == IsHashTickIntervalMethod)
                {
                    yield return instruction;
                    foreach (var codeInstruction in LogMessage("IsHashIntervalTick true"))
                        yield return codeInstruction;
                    continue;
                }
                
            }
            
            // Check if we're past the IsHashIntervalTick if block
            if (instruction.opcode == OpCodes.Ldarg_0 && previous != null)
            {
                if (previous.opcode == OpCodes.Stfld &&
                    previous.operand is FieldInfo field &&
                    field == Level0HintEffecterField)
                {
                    yield return instruction;

                    foreach (var codeInstruction in LogMessage("End of IsHashIntervalTick block"))
                        yield return codeInstruction;
                    continue;
                }
            }

            // Check if Find.Anomaly.Level is zero
            if (instruction.opcode == OpCodes.Brtrue_S && previous != null)
            {
                if (previous.opcode == OpCodes.Callvirt &&
                    previous.operand is MethodInfo method &&
                    method == GetLevelMethod)
                {
                    yield return instruction;
                    
                    foreach (var codeInstruction in LogMessage("Anomaly level is zero"))
                        yield return codeInstruction;
                    
                    continue;
                }
            }
            
            // Check if Find.Anomaly.Level is not zero || IsReserved false
            if (instruction.opcode == OpCodes.Ldarg_0 && next != null)
            {
                if (next.opcode == OpCodes.Ldfld &&
                    next.operand is FieldInfo field &&
                    field == Level0HintEffecterField)
                {
                    yield return instruction;
                    
                    foreach (var codeInstruction in LogMessage("Anomaly level is not zero || IsReserved false"))
                        yield return codeInstruction;
                    
                    continue;
                }
            }

            // Check if IsReserved returns false
            if (instruction.opcode == OpCodes.Brtrue_S && previous != null)
            {
                if (previous.opcode == OpCodes.Callvirt &&
                    previous.operand is MethodInfo method &&
                    method == IsReservedMethod)
                {
                    yield return instruction;
                    
                    foreach (var codeInstruction in LogMessage("IsReserved false"))
                        yield return codeInstruction;
                    
                    continue;
                }
            } 
            
            
            
            
            
            
            // Check if we're about to return
            if (instruction.opcode == OpCodes.Ret)
            {
                foreach (var codeInstruction in LogMessage("End of tick"))
                    yield return codeInstruction;
            }

            // No special handling for this instruction 
            yield return instruction;
        }
    }

    static IEnumerable<CodeInstruction> LogMessage(String message)
    {
        Log.Message("Inserting: " + message);
        yield return new CodeInstruction(OpCodes.Ldstr, message);
        yield return new CodeInstruction(OpCodes.Call, LogMessageMethod);
    }

    static void PrettyPleaseLog(String message)
    {
        Log.Message("Please oh lord dead god be here");
    }
}

[HarmonyPatch(typeof(Building_VoidMonolith), "IsHashIntervalTick")]
public static class IsHashIntervalTick_Patch
{
    static void PostFix()
    {
        Log.Message("Is hash interval tick");
    }
}

[HarmonyPatch(typeof(Building_VoidMonolith), "Activate")]
public static class Activate_Patch
{
    static void PostFix()
    {
        Log.Message("Activate");
    }
}


