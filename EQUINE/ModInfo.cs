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

namespace EQUINE
{
    public class ModInfo
    {
        public int _modID { get; set; }
        public string _game { get; set; }
        public string _modName { get; set; }
        public string _description { get; set; }
        public string _author { get; set; }
        public string _modVersion { get; set; }
        public string _website { get; set; }
        public string _DL1 { get; set; }
        public string _DL2 { get; set; }
        public string _md5 { get; set; }
        public string _startExe0 { get; set; }
        public string _startExe1 { get; set; }
        public string _beforeInstallMessage { get; set; }
        public string _afterInstallMessage { get; set; }
        public bool _diabdatRequired { get; set; }
        public string _modExecutable { get; set; }
    }
}
