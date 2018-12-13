using MassiveGame.Core.GameCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MassiveGame.UiWindow
{
    public static class UiMouseEventHandlerHalper
    {
        public static Point GetChildLocationOffsetAtWindow(Control childControl, Control windowControl)
        {
            Point parentLocationTotal = new Point(0, 0);

            Control parentControl = childControl.Parent;
            while (parentControl != windowControl)
            {
                parentLocationTotal.Offset(parentControl.Location);
                parentControl = parentControl.Parent;
            }

            // If child's location has offset then add it to total
            if (childControl.Location.X != 0 || childControl.Location.Y != 0)
            {
                parentLocationTotal.Offset(childControl.Location);
            }

            return parentLocationTotal;
        }

        public static void DoCorrectRotation(MouseEventArgs mouseEvent, BaseCamera camera, Control childControl, Control windowControl)
        {
            Point childOffset = GetChildLocationOffsetAtWindow(childControl, windowControl);

            childOffset.Offset(mouseEvent.Location);

            if (EngineStatics.PrevCursorPosition != mouseEvent.Location)
            {
                EngineStatics.PrevCursorPosition = mouseEvent.Location;
                camera.RotateFacade(childOffset, new Point(childControl.Width, childControl.Height));
            }
        }
    }
}
