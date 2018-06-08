using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Settings
{
    #region PostFxSettings

    [Flags]
    public enum PostProcessFlag
    {
        PostFxDisable = 0x0001,
        GrEffectsDisable = 0x0002,
        PostFx_and_GrEffects_Disable = PostFxDisable | GrEffectsDisable,
        PostFxEnable = 0x0010,
        GrEffectsEnable = 0x0020,
        PostFx_and_GrEffects_Enable = PostFxEnable | GrEffectsEnable
    }

    #endregion
}
