﻿using System;
using System.Collections.Generic;

namespace MassiveGame.Core.RenderCore.Shadows
{
    public class ShadowMapCache
    {
        public enum CacheSlot
        {
            Slot1,
            Slot2,
            Slot3
        }

        private Dictionary<CacheSlot, Int32> cacheMap;

        public ShadowMapCache()
        {
            cacheMap = new Dictionary<CacheSlot, Int32>(3);
        }

        public void SetShadowMap(CacheSlot slot, Int32 cacheMapHandler)
        {
            if (cacheMap[slot] > 0)
            {
                throw new FieldAccessException("This cache map is already busy, first release it.");
            }
            cacheMap[slot] = cacheMapHandler;
        }

        public Int32 GetShadowMap(CacheSlot slot)
        {
            return cacheMap[slot];
        }
    }
}
