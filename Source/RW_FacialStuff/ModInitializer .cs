﻿using UnityEngine;
using Verse;
using CommunityCoreLibrary;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace RW_FacialStuff
{

    public class ModInitializer : ITab
    {
        protected GameObject modInitializerControllerObject;

        public ModInitializer()
        {
            modInitializerControllerObject = new GameObject("BeardyFaces");
            modInitializerControllerObject.AddComponent<ModInitializerBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)modInitializerControllerObject);

            MethodInfo method = typeof(GraphicDatabaseUtility).GetMethod("GraphicNamesInFolder", BindingFlags.Static | BindingFlags.Public);
            MethodInfo method2 = typeof(ModInitializer).GetMethod("GraphicNamesInFolder", BindingFlags.Static | BindingFlags.Public);
            Detours.TryDetourFromTo(method, method2);
        }

        public static IEnumerable<string> GraphicNamesInFolder(string folderPath)
        {
            IEnumerable<Texture2D> allInFolder = ContentFinder<Texture2D>.GetAllInFolder(folderPath);
            List<string> list = new List<string>();
            using (IEnumerator<Texture2D> enumerator = allInFolder.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<string> list2 = enumerator.Current.name.Split("_".ToCharArray()).ToList();
                    if (list2.Count > 4)
                    {
                        Log.Error("Cannot load assets with >3 pieces.");
                    }
                    else if (list2.Count == 1 && !list.Contains(list2[0]))
                    {
                        list.Add(list2[0]);
                    }
                    else
                    {
                        list2.Remove(list2.Last());
                        string item = string.Join("_", list2.ToArray());
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }


        protected override void FillTab() { }
    }


    class ModInitializerBehaviour : MonoBehaviour
    {
        protected bool _reinjectNeeded;
        protected float _reinjectTime;

        public void OnLevelWasLoaded(int level)
        {
            _reinjectNeeded = true;
            if (level >= 0)
                _reinjectTime = 1;
            else
                _reinjectTime = 0;
        }

        public void FixedUpdate()
        {
            if (_reinjectNeeded)
            {
                _reinjectTime -= Time.fixedDeltaTime;

                if (_reinjectTime <= 0)
                {
                    _reinjectNeeded = false;
                    _reinjectTime = 0;

#if LOG
                    Log.Message("AutoEquip Injected");
#endif
                    MapComponent_FacialStuff component = MapComponent_FacialStuff.Get;
                }
            }
        }


        public void Start()
        {


            MethodInfo coreMethod = typeof(Verse.PawnGraphicSet).GetMethod("ResolveAllGraphics", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo autoEquipMethod = typeof(RW_FacialStuff.PawnGraphicHairSet).GetMethod("ResolveAllGraphicsModded", BindingFlags.Instance | BindingFlags.Public);

                     MethodInfo coreMethod20 = typeof(GraphicDatabaseHeadRecords).GetMethod("BuildDatabaseIfNecessary", BindingFlags.Instance | BindingFlags.NonPublic);
                     MethodInfo autoEquipMethod20 = typeof(RW_FacialStuff.GraphicDatabaseModdedHeadRecords).GetMethod("BuildDatabaseIfNecessary", BindingFlags.Instance | BindingFlags.NonPublic);

      //    MethodInfo coreMethod2 = typeof(RimWorld.Pawn_StoryTracker).GetMethod("ExposeData", BindingFlags.CreateInstance | BindingFlags.Public);
      //    MethodInfo autoEquipMethod2 = typeof(RW_FacialStuff.Pawn_StoryTrackerModded).GetMethod("ExposeData", BindingFlags.CreateInstance | BindingFlags.Public);


          MethodInfo coreMethod2 = typeof(Verse.GraphicDatabaseHeadRecords).GetMethod("GetHeadRandom", BindingFlags.Static | BindingFlags.Public);
          MethodInfo autoEquipMethod2 = typeof(RW_FacialStuff.GraphicDatabaseModdedHeadRecords).GetMethod("GetHeadRandomUnmodded", BindingFlags.Static | BindingFlags.Public);


            //       MethodInfo coreMethod2 = typeof(Verse.PawnRenderer).GetMethod("RenderPawnInternal",
            //           BindingFlags.Instance | BindingFlags.NonPublic,
            //           Type.DefaultBinder, new[] { typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode) }, null);
            //
            //       MethodInfo autoEquipMethod2 = typeof(FS_PawnRenderer).GetMethod("RenderPawnInternal", BindingFlags.Instance | BindingFlags.NonPublic,
            //           Type.DefaultBinder, new[] { typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode) }, null);

            try
            {
                Detours.TryDetourFromTo(coreMethod, autoEquipMethod);

                Detours.TryDetourFromTo(coreMethod2, autoEquipMethod2);

            //    Detours.TryDetourFromTo(coreMethod20, autoEquipMethod20);
            }
            catch (Exception)
            {
                Log.Error("Could not Detour Graphics.");
                throw;
            }


        }
    }
}
