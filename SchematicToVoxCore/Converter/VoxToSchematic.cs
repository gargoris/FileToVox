﻿using FileToVox.Extensions;
using FileToVoxCore.Extensions;
using FileToVoxCore.Schematics;
using FileToVoxCore.Schematics.Tools;
using FileToVoxCore.Utils;
using FileToVoxCore.Vox;
using MoreLinq;

namespace FileToVox.Converter
{
    public class VoxToSchematic : AbstractToSchematic
    {
        private VoxModel mVoxModel;
        public VoxToSchematic(string path) : base(path)
        {
            VoxReader reader = new VoxReader();
            mVoxModel = reader.LoadModel(path);
        }

        public override Schematic WriteSchematic()
        {
            Schematic schematic = new Schematic();
            FileToVoxCore.Drawing.Color[] colorsPalette = mVoxModel.Palette;
            using (ProgressBar progressbar = new ProgressBar())
            {
                int minX = (int)mVoxModel.TransformNodeChunks.MinBy(t => t.TranslationAt().X).First().TranslationAt().X;
                int minY = (int)mVoxModel.TransformNodeChunks.MinBy(t => t.TranslationAt().Y).First().TranslationAt().Y;
                int minZ = (int)mVoxModel.TransformNodeChunks.MinBy(t => t.TranslationAt().Z).First().TranslationAt().Z;

                for (int i = 0; i < mVoxModel.VoxelFrames.Count; i++)
                {
                    VoxelData data = mVoxModel.VoxelFrames[i];
                    Vector3 worldPositionFrame = mVoxModel.TransformNodeChunks[i + 1].TranslationAt();

                    if (worldPositionFrame == Vector3.zero)
                        continue;

                    for (int y = 0; y < data.VoxelsTall; y++)
                    {
                        for (int z = 0; z < data.VoxelsDeep; z++)
                        {
                            for (int x = 0; x < data.VoxelsWide; x++)
                            {
                                int indexColor = data.Get(x, y, z);
                                FileToVoxCore.Drawing.Color color = colorsPalette[indexColor];
                                if (color != FileToVoxCore.Drawing.Color.Empty)
                                {
                                    schematic.AddVoxel((int)(z + worldPositionFrame.X - minX), (int)(y + (worldPositionFrame.Z - minZ)), (int)(x + worldPositionFrame.Y - minY), color.ColorToUInt());
                                }
                            }
                        }
                    }
                    progressbar.Report(i / (float)mVoxModel.VoxelFrames.Count);
                }
            }


            return schematic;
        }
    }
}
