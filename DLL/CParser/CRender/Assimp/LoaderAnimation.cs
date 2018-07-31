using Assimp;
using System;
using System.Collections.Generic;

namespace CParser.Assimp
{
    public class LoaderAnimation
    {
        public double AnimationDuration { set; get; }
        public string Name { set; get; }
        public List<LoaderBoneFrameCollection> FramesBoneCollection { set; get; }

        public LoaderAnimation(Animation animation)
        {
            Name = animation.Name;
            AnimationDuration = animation.DurationInTicks;
            FramesBoneCollection = new List<LoaderBoneFrameCollection>(animation.NodeAnimationChannelCount);
            foreach (var channel in animation.NodeAnimationChannels)
            {
                var boneFramesCollection = new LoaderBoneFrameCollection(channel.NodeName);
               
                Int32 keysNumber = Math.Max(channel.PositionKeyCount, Math.Max(channel.RotationKeyCount, channel.ScalingKeyCount));

                for (Int32 i = 0; i < keysNumber; i++)
                {
                    Vector3D? translation = null, scaling = null;
                    Quaternion? rotation = null;
                    double time = -1.0;

                    if (channel.HasPositionKeys)
                    {
                        if (channel.PositionKeyCount > i)
                        {
                            translation = channel.PositionKeys[i].Value;
                            time = channel.PositionKeys[i].Time;
                        }
                    }
                    if (channel.HasScalingKeys)
                    {
                        if (channel.ScalingKeyCount > i)
                        {
                            scaling = channel.ScalingKeys[i].Value;
                            time = channel.ScalingKeys[i].Time;
                        }
                    }
                    if (channel.HasRotationKeys)
                    {
                        if (channel.RotationKeyCount > i)
                        {
                            rotation = channel.RotationKeys[i].Value;
                            time = channel.RotationKeys[i].Time;
                        }
                    }

                    boneFramesCollection.AddFrame(new LoaderAnimationFrame(time, translation, rotation, scaling));
                }

                FramesBoneCollection.Add(boneFramesCollection);
            }
        }
    }
}
