﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FacialStuff.DefOfs;
using FacialStuff.Defs;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FacialStuff.AnimatorWindows
{
    public class MainTabWindow_WalkAnimator : MainTabWindow_BaseAnimator
    {

        #region Public Fields

        public static bool Equipment;
        public static float HorHeadOffset;
        public static float VerHeadOffset;

        #endregion Public Fields

        #region Public Properties
        public override void SetKeyframes()
        {
            PawnKeyframes = EditorWalkcycle.keyframes;
            this.Label = EditorWalkcycle.LabelCap;
        }
        [NotNull]
        public static WalkCycleDef EditorWalkcycle { get; private set; } = WalkCycleDefOf.Biped_Walk;


        #endregion Public Properties

        #region Private Properties

        // public static float verHeadOffset;
        #endregion Private Properties

        #region Public Methods

        protected override void BuildEditorCycle()
        {
            base.BuildEditorCycle();
            GameComponent_FacialStuff.BuildWalkCycles(EditorWalkcycle);

        }

        protected override void SetCurrentCycle()
        {
            BodyAnimDef anim = this.CompAnim.BodyAnim;
            if (anim != null && anim.walkCycles.Any())
            {
                EditorWalkcycle =
                anim.walkCycles.FirstOrDefault().Value;
            }
        }

        // public static float horHeadOffset;
        protected override void DoBasicSettingsMenu(Listing_Standard listing)
        {
            base.DoBasicSettingsMenu(listing);


          //  listing.CheckboxLabeled("Moving", ref IsMoving);

            // listing_Standard.CheckboxLabeled("Equipment", ref Equipment);

            // listing_Standard.Label(horHeadOffset.ToString("N2") + " - " + verHeadOffset.ToString("N2"));
            // horHeadOffset = listing_Standard.Slider(horHeadOffset, -1f, 1f);
            // verHeadOffset = listing_Standard.Slider(verHeadOffset, -1f, 1f);
            listing.Label(this.BodyAnimDef.offCenterX.ToString("N2"));
            this.BodyAnimDef.offCenterX = listing.Slider(this.BodyAnimDef.offCenterX, -0.2f, 0.2f);


            if (listing.ButtonText(EditorWalkcycle.LabelCap))
            {
                List<string> exists = new List<string>();
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                this.BodyAnimDef.walkCycles.Clear();

                foreach (WalkCycleDef walkcycle in (from bsm in DefDatabase<WalkCycleDef>.AllDefs
                                                    orderby bsm.LabelCap
                                                    select bsm)
                                                  .TakeWhile(current => this.BodyAnimDef.WalkCycleType != "None")
                                                  .Where(current => current.WalkCycleType ==
                                                                    this.BodyAnimDef.WalkCycleType))
                {
                    list.Add(new FloatMenuOption(walkcycle.LabelCap, delegate { EditorWalkcycle = walkcycle; }));
                    exists.Add(walkcycle.locomotionUrgency.ToString());
                    this.BodyAnimDef.walkCycles.Add(walkcycle.locomotionUrgency, walkcycle);
                }

                string[] names = Enum.GetNames(typeof(LocomotionUrgency));
                for (int index = 0; index < names.Length; index++)
                {
                    string name = names[index];
                    LocomotionUrgency myenum = (LocomotionUrgency)Enum.ToObject(typeof(LocomotionUrgency), index);

                    if (exists.Contains(myenum.ToString()))
                    {
                        continue;
                    }

                    list.Add(
                             new FloatMenuOption(
                                                 "Add new " + this.BodyAnimDef.WalkCycleType + "_" + myenum,
                                                 delegate
                                                 {
                                                     WalkCycleDef newCycle = new WalkCycleDef();
                                                     newCycle.defName =
                                                     newCycle.label =
                                                     this.BodyAnimDef.WalkCycleType + "_" + name;
                                                     newCycle.locomotionUrgency = myenum;
                                                     newCycle.WalkCycleType = this.BodyAnimDef.WalkCycleType;
                                                     GameComponent_FacialStuff.BuildWalkCycles(newCycle);
                                                     EditorWalkcycle = newCycle;

                                                     this.BodyAnimDef.walkCycles.Add(myenum, newCycle);
                                                 }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }

            if (listing.ButtonText("This pawn is using: " + this.BodyAnimDef.WalkCycleType))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();

                List<WalkCycleDef> listy = DefDatabase<WalkCycleDef>.AllDefsListForReading;

                List<string> stringsy = new List<string>();

                foreach (WalkCycleDef cycleDef in listy)
                {
                    if (!stringsy.Contains(cycleDef.WalkCycleType))
                    {
                        stringsy.Add(cycleDef.WalkCycleType);
                    }
                }

                foreach (string s in stringsy)
                {
                    list.Add(new FloatMenuOption(s, delegate { this.BodyAnimDef.WalkCycleType = s; }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }



            listing.Gap();
            string configFolder = DefPath;
            if (listing.ButtonText("Export BodyDef"))
            {
                string filePath = configFolder + "/BodyAnimDefs/" + this.BodyAnimDef.defName + ".xml";

                Find.WindowStack.Add(
                                     Dialog_MessageBox.CreateConfirmation(
                                                                          "Confirm overwriting " +
                                                                          filePath,
                                                                          delegate
                                                                          {
                                                                              ExportAnimDefs.Defs animDef =
                                                                              new ExportAnimDefs.Defs(this.BodyAnimDef);

                                                                              DirectXmlSaver.SaveDataObject(
                                                                                                            animDef,
                                                                                                            filePath);
                                                                          },
                                                                          true));

                // BodyAnimDef animDef = this.bodyAnimDef;
            }

            if (listing.ButtonText("Export WalkCycle"))
            {
                string path = configFolder + "/WalkCycleDefs/" + EditorWalkcycle.defName + ".xml";

                Find.WindowStack.Add(
                                     Dialog_MessageBox.CreateConfirmation(
                                                                          "Confirm overwriting " + path,
                                                                          delegate
                                                                          {
                                                                              ExportWalkCycleDefs.Defs cycle =
                                                                              new ExportWalkCycleDefs.
                                                                              Defs(EditorWalkcycle);

                                                                              DirectXmlSaver.SaveDataObject(
                                                                                                            cycle,
                                                                                                            path);
                                                                          },
                                                                          true));
            }

        }

        public override void PreOpen()
        {
            base.PreOpen();
           // IsMoving = true;
        }

        protected override void SetAnimOpen()
        {
            base.SetAnimOpen();
            this.CompAnim.AnimatorWalkOpen = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);
            if (GUI.changed)
            {
                if (!this.Loop)
                {
                    GameComponent_FacialStuff.BuildWalkCycles();
                }
            }
        }

        protected override void DrawBodySettingsEditor(Rot4 rotation)
        {
            Rect sliderRect = new Rect(0, 0, this.SliderWidth, 40f);

            // this.DrawBodyStats("legLength", ref bodyAnimDef.legLength, ref sliderRect);
            // this.DrawBodyStats("hipOffsetVerticalFromCenter",
            // ref bodyAnimDef.hipOffsetVerticalFromCenter, ref sliderRect);
            Vector3 shoulderOffset = this.BodyAnimDef.shoulderOffsets[rotation.AsInt];

            if (shoulderOffset.y == 0f)
            {
                if (rotation == Rot4.West)
                {
                    shoulderOffset.y = -0.025f;
                }
                else
                {
                    shoulderOffset.y = 0.025f;
                }
            }

            bool front = shoulderOffset.y > 0;

            if (rotation == Rot4.West)
            {
                front = shoulderOffset.y < 0;
            }

            this.DrawBodyStats("shoulderOffsetX", ref shoulderOffset.x, ref sliderRect);
            this.DrawBodyStats("shoulderOffsetZ", ref shoulderOffset.z, ref sliderRect);
            // this.DrawBodyStats("shoulderFront",   ref front,            ref sliderRect);

            Vector3 hipOffset = this.BodyAnimDef.hipOffsets[rotation.AsInt];
            if (hipOffset.y == 0f)
            {
                if (rotation == Rot4.West)
                {
                    hipOffset.y = -0.025f;
                }
                else
                {
                    hipOffset.y = 0.025f;
                }
            }

            bool hipFront = hipOffset.y > 0;
            if (rotation == Rot4.West)
            {
                hipFront = hipOffset.y < 0;
            }

            this.DrawBodyStats("hipOffsetX", ref hipOffset.x, ref sliderRect);
            this.DrawBodyStats("hipOffsetZ", ref hipOffset.z, ref sliderRect);
            // this.DrawBodyStats("hipFront",   ref hipFront,    ref sliderRect);

            if (GUI.changed)
            {
                this.SetNewVector(rotation, shoulderOffset, this.BodyAnimDef.shoulderOffsets, front);
                this.SetNewVector(rotation, hipOffset, this.BodyAnimDef.hipOffsets, hipFront);
            }

            this.DrawBodyStats("armLength", ref this.BodyAnimDef.armLength, ref sliderRect);
            this.DrawBodyStats("extraLegLength", ref this.BodyAnimDef.extraLegLength, ref sliderRect);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected override void DrawKeyframeEditor(Rect controller, Rot4 rotation)
        {
            if (this.CurrentFrame== null)
            {
                return;
            }

            Rect leftController = controller.LeftHalf();
            Rect rightController = controller.RightHalf();
            leftController.xMax -= this.Spacing;

            rightController.xMin += this.Spacing;
            {
                GUI.BeginGroup(leftController);
                Rect editorRect = new Rect(0f, 0f, leftController.width, 56f);

                // Dictionary<int, float> keysFloats = new Dictionary<int, float>();

                // // Get the next keyframe
                // for (int i = 0; i < frames.Count; i++)
                // {
                // float? footPositionX = frames[i].FootPositionX;
                // if (!footPositionX.HasValue)
                // {
                // continue;
                // }
                // keysFloats.Add(frames[i].KeyIndex, footPositionX.Value);
                // }
                List<int> framesAt;
                List<PawnKeyframe> frames = PawnKeyframes;
                WalkCycleDef walkcycle = EditorWalkcycle;
                {
                    framesAt = (from keyframe in frames where keyframe.FootPositionX.HasValue select keyframe.KeyIndex)
                   .ToList();

                    this.SetPosition(
                                     ref this.CurrentFrame.FootPositionX,
                                     ref editorRect,
                                     walkcycle.FootPositionX,
                                     "FootPosX",
                                     framesAt);

                    framesAt = (from keyframe in frames where keyframe.FootPositionZ.HasValue select keyframe.KeyIndex)
                   .ToList();

                    this.SetPosition(
                                     ref this.CurrentFrame.FootPositionZ,
                                     ref editorRect,
                                     walkcycle.FootPositionZ,
                                     "FootPosY",
                                     framesAt);

                    framesAt = (from keyframe in frames where keyframe.FootAngle.HasValue select keyframe.KeyIndex)
                   .ToList();

                    this.SetAngle(
                                  ref this.CurrentFrame.FootAngle,
                                  ref editorRect,
                                  walkcycle.FootAngle,
                                  "FootAngle",
                                  framesAt);

                    framesAt = (from keyframe in frames
                                where keyframe.HipOffsetHorizontalX.HasValue
                                select keyframe.KeyIndex).ToList();

                    this.SetPosition(
                                     ref this.CurrentFrame.HipOffsetHorizontalX,
                                     ref editorRect,
                                     walkcycle.HipOffsetHorizontalX,
                                     "HipOffsetHorizontalX",
                                     framesAt);

                    // Quadruped
                }

                // else
                // {
                // framesAt = (from keyframe in frames
                // where keyframe.FootPositionVerticalZ.HasValue
                // select keyframe.KeyIndex).ToList();
                // this.SetPosition(
                // ref thisFrame.FootPositionVerticalZ,
                // ref editorRect,
                // EditorWalkcycle.FootPositionVerticalZ,
                // "FootPosVerticalY", framesAt);
                // }
                GUI.EndGroup();

                GUI.BeginGroup(rightController);

                editorRect.x = 0f;
                editorRect.y = 0f;

                if (this.CompAnim.Props.bipedWithHands)
                {
                    this.SetAngleShoulder(ref walkcycle.shoulderAngle, ref editorRect, "ShoulderAngle");

                    framesAt =
                    (from keyframe in frames where keyframe.HandsSwingAngle.HasValue select keyframe.KeyIndex)
                   .ToList();

                    this.SetAngle(
                                  ref this.CurrentFrame.HandsSwingAngle,
                                  ref editorRect,
                                  walkcycle.HandsSwingAngle,
                                  "HandSwing",
                                  framesAt);


                }

                if (rotation.IsHorizontal)
                {
                    if (this.CompAnim.Props.quadruped)
                    {
                        framesAt = (from keyframe in frames
                                    where keyframe.FrontPawPositionX.HasValue
                                    select keyframe.KeyIndex).ToList();
                        this.SetPosition(
                                         ref this.CurrentFrame.FrontPawPositionX,
                                         ref editorRect,
                                         walkcycle.FrontPawPositionX,
                                         "FrontPawPositionX",
                                         framesAt);

                        framesAt = (from keyframe in frames
                                    where keyframe.FrontPawPositionZ.HasValue
                                    select keyframe.KeyIndex).ToList();

                        this.SetPosition(
                                         ref this.CurrentFrame.FrontPawPositionZ,
                                         ref editorRect,
                                         walkcycle.FrontPawPositionZ,
                                         "FrontPawPositionZ",
                                         framesAt);

                        framesAt = (from keyframe in frames
                                    where keyframe.FrontPawAngle.HasValue
                                    select keyframe.KeyIndex).ToList();

                        this.SetAngle(
                                      ref this.CurrentFrame.FrontPawAngle,
                                      ref editorRect,
                                      walkcycle.FrontPawAngle,
                                      "FrontPawAngle",
                                      framesAt);
                    }

                    framesAt = (from keyframe in frames where keyframe.BodyAngle.HasValue select keyframe.KeyIndex)
                   .ToList();

                    this.SetAngle(
                                  ref this.CurrentFrame.BodyAngle,
                                  ref editorRect,
                                  walkcycle.BodyAngle,
                                  "BodyAngle",
                                  framesAt);
                }
                else
                {
                    if (this.CompAnim.Props.bipedWithHands)
                    {
                        // framesAt = (from keyframe in frames
                        // where keyframe.HandsSwingPosVertical.HasValue
                        // select keyframe.KeyIndex).ToList();
                        // this.SetPosition(
                        // ref thisFrame.HandsSwingPosVertical,
                        // ref editorRect,
                        // EditorWalkcycle.HandsSwingPosVertical,
                        // "HandsSwingPosVertical", framesAt);
                    }

                    // framesAt = (from keyframe in frames
                    // where keyframe.FrontPawPositionVerticalZ.HasValue
                    // select keyframe.KeyIndex).ToList();
                    // if (this.CompAnim.Props.quadruped)
                    // {
                    // this.SetPosition(
                    // ref thisFrame.FrontPawPositionVerticalZ,
                    // ref editorRect,
                    // EditorWalkcycle.FrontPawPositionVerticalZ,
                    // "FrontPawPosVerticalY", framesAt);
                    // }
                    // framesAt = (from keyframe in frames
                    // where keyframe.BodyOffsetVerticalZ.HasValue
                    // select keyframe.KeyIndex).ToList();
                    // this.SetPosition(
                    // ref thisFrame.BodyOffsetVerticalZ,
                    // ref editorRect,
                    // EditorWalkcycle.BodyOffsetVerticalZ,
                    // "BodyOffsetVerticalZ", framesAt);
                    framesAt = (from keyframe in frames
                                where keyframe.BodyAngleVertical.HasValue
                                select keyframe.KeyIndex).ToList();
                    this.SetAngle(
                                  ref this.CurrentFrame.BodyAngleVertical,
                                  ref editorRect,
                                  walkcycle.BodyAngleVertical,
                                  "BodyAngleVertical",
                                  framesAt);
                }
                framesAt = (from keyframe in frames
                            where keyframe.ShoulderOffsetHorizontalX.HasValue
                            select keyframe.KeyIndex).ToList();
                this.SetPosition(
                                 ref this.CurrentFrame.ShoulderOffsetHorizontalX,
                                 ref editorRect,
                                 walkcycle.ShoulderOffsetHorizontalX,
                                 "ShoulderOffsetHorizontalX",
                                 framesAt);
                framesAt =
                (from keyframe in frames where keyframe.BodyOffsetZ.HasValue select keyframe.KeyIndex).ToList();

                this.SetPosition(
                                 ref this.CurrentFrame.BodyOffsetZ,
                                 ref editorRect,
                                 walkcycle.BodyOffsetZ,
                                 "BodyOffsetZ",
                                 framesAt);

                GUI.EndGroup();
            }
        }

        protected override void FindRandomPawn()
        {
            base.FindRandomPawn();
            BodyAnimDef anim = this.CompAnim.BodyAnim;
            if (anim != null && anim.walkCycles.Any())
            {
                EditorWalkcycle = anim.walkCycles.FirstOrDefault().Value;
            }
                this.CompAnim.AnimatorWalkOpen = true;
        }

        #endregion Public Methods

        #region Private Methods

        private void DrawBodyStats(string label, ref float value, ref Rect sliderRect)
        {
            float left = -1.5f;
            float right = 1.5f;
            value = Widgets.HorizontalSlider(
                                                   sliderRect,
                                                   value,
                                                   left,
                                                   right,
                                                   false,
                                                   label + ": " + value,
                                                   left.ToString(),
                                                   right.ToString(),
                                                   0.025f);

            sliderRect.y += sliderRect.height + 8f;
        }

        #endregion Private Methods
    }
}