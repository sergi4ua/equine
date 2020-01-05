/*Copyright(C) 2018 Sergi4UA

This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EQUINE
{
    public class GlobalVariableContainer
    {
        public static string Version = Application.ProductVersion.ToString();
        public static string AppName = "EQUINE for Diablo 1 - Version 1.0";
        public static bool DIABDATPresent = false;

        public static string[] Messages =
        {
               "We don't have phones... we have PCs with ZeroTier!",
               "AHHH! Fresh meat!",
               "Play D1Legit!",
               "Tiny pancakes are evil.",
               "Make Diablo great again!",
               "And remember - no duping!",
               "Thinking has no cap",
               "Have any good items, m8?",
               "Made with ♥",
               "*mage death sound*",
               "Abandon your foolish quest and download some mods!",
               "Mods. Mods never changing.",
               "The Phrozen Keep is Phrozen.",
               "Hey! Look out for that yellow zombie thing.",
               "If you like Diablo 1, buy it on GOG!",
               "If you like Hellfire, buy it on GOG!",
               "The Hell is hellish enough.",
               "Stop reading this and download some mods already!",
               "Ha-ha! Your items have been doubled!",
               "*rogue death sound*",
               "*warrior death sound*",
               "*potion sound*",
               "Arcane knowledge gained!",
               "Made with ♥ and sausage",
               "Your rectum has been slain."
        };

        public static bool skipUpdates = false;
    }
}
