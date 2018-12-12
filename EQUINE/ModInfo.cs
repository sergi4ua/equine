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

using System.Collections.Generic;

namespace EQUINE
{
    public class ModInfo
    {
        public string ModId { get; set; }
        public string ModName { get; set; }
        public string Game { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string ModVersion { get; set; }
        public string WebSite { get; set; }
        public string DL { get; set; }
        public string DL2 { get; set; }
        public string md5 { get; set; }
        public string RunExeAfterInstall { get; set; }
        public string RunExeAfterInstall2 { get; set; }
        public string BeforeInstallMessage { get; set; }
        public string AfterInstallMessage { get; set; }
        public string DiabdatRequired { get; set; }
        public string Executable { get; set; }
    }

    public class RootObject
    {
        public List<ModInfo> ModInfo { get; set; }
    }
}
