﻿namespace FacialStuff
{
    using FacialStuff.Graphics_FS;

    using JetBrains.Annotations;

    using Verse;

    internal class FaceGraphicParts
    {
        #region Public Fields

        public Graphic BrowGraphic;

        public Graphic DeadEyeGraphic;

        public Graphic_Multi_NaturalEyes EyeLeftClosedGraphic;

        public Graphic_Multi_NaturalEyes EyeLeftGraphic;

        [CanBeNull]
        public Graphic_Multi_AddedHeadParts EyeLeftPatchGraphic;

        public Graphic_Multi_NaturalEyes EyeRightClosedGraphic;

        public Graphic_Multi_NaturalEyes EyeRightGraphic;

        [CanBeNull]
        public Graphic_Multi_AddedHeadParts EyeRightPatchGraphic;

        [CanBeNull]
        public Graphic_Multi_NaturalHeadParts JawGraphic;

        public Graphic MainBeardGraphic;

        [CanBeNull]
        public Graphic MoustacheGraphic;

        public Graphic_Multi_NaturalHeadParts MouthGraphic;

        public Graphic RottingWrinkleGraphic;

        public Graphic WrinkleGraphic;

        #endregion Public Fields
    }
}