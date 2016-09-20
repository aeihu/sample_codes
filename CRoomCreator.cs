using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Threading.Tasks;

namespace RoomGenerator
{
    class CMovement
    {
        int _start = 0;
        int _end = 0;
        public int _Start
        {
            get {
                return _start;
            }
            set {
                _start = value;
            }
        }
        public int _End
        {
            get{
                return _end;
            }
            set{
                _end = value;
            }
        }

        public CMovement(int start, int end)
        {
            _Start = start;
            _End = end;
        }
    }

    class CMove : CMovement
    {
        List<CMovement> _jumpList;

        public bool AddJump(int start, int time)
        {
            if (start >= 0 && time > 0 && start + time <= _End) {
                _jumpList.Add(new CMovement(start, start + time));
                return true;
            }

            return false;
        }

        public CMove(int start, int end)
            : base(start, end)
        { }
    }

    class CRoomCreator
    {
        private static System.Random _random = new System.Random();
        public class CVector2i : IEquatable<CVector2i>  
        {
            public int _X
            {
                get;
                set;
            }
            public int _Y
            {
                get;
                set;
            }
            public static CVector2i operator +(CVector2i lvec, CVector2i rvec)
            {
                CVector2i __result = new CVector2i();
                __result._X = lvec._X + rvec._X;
                __result._Y = lvec._Y + rvec._Y;
                return __result;
            }
            public static CVector2i operator -(CVector2i lvec, CVector2i rvec)
            {
                CVector2i __result = new CVector2i();
                __result._X = lvec._X - rvec._X;
                __result._Y = lvec._Y - rvec._Y;
                return __result;
            } 
            public bool Equals(CVector2i other)  
            {  
                if (System.Object.ReferenceEquals(other, null)) return false;  
                if (System.Object.ReferenceEquals(this, other)) return true;  
  
                return _X.Equals(other._X) && _Y.Equals(other._Y);  
            }
            public override int GetHashCode()
            {
 
                int hash_X = _X.GetHashCode();
                int hash_Y = _Y.GetHashCode();

                return hash_X ^ hash_Y;
            } 

            public CVector2i(int x, int y)
            {
                _X = x;
                _Y = y;
            }
            public CVector2i()
            {
                _Y = _X = 0;
            }
        }

        int _jumpMaxHeight = 2;
        int _wall = 2;
        int _width = 0;
        int _height = 0;
        int[,] _space = null;
        CVector2i[] _entries;
        CVector2i[] _exits;
        List<CVector2i> _checkpionts = new List<CVector2i>();
        List<CVector2i> _path = new List<CVector2i>();

        public void ExportTMX(string path)
        {
            ResetSpace(20, 15);
            XmlDocument __xmlDoc = new XmlDocument();   
            //建立Xml的定义声明   
            XmlDeclaration __dec = __xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);   
            __xmlDoc.AppendChild(__dec);   
            //创建根节点   
            XmlElement __map = __xmlDoc.CreateElement("map");
            __xmlDoc.AppendChild(__map);   
            __map.SetAttribute("version", "1.0");
            __map.SetAttribute("orientation", "orthogonal");
            __map.SetAttribute("width", "20");
            __map.SetAttribute("height", "15");
            __map.SetAttribute("tilewidth", "32");
            __map.SetAttribute("tileheight", "32");

            XmlElement __tileset = __xmlDoc.CreateElement("tileset"); 
            __map.AppendChild(__tileset);
            __tileset.SetAttribute("firstgid", "1");
            __tileset.SetAttribute("name", "tileset");
            __tileset.SetAttribute("tilewidth", "32");
            __tileset.SetAttribute("tileheight", "32");

            XmlElement __image = __xmlDoc.CreateElement("image"); 
            __tileset.AppendChild(__image);
            __image.SetAttribute("source", "tileset.png");
            __image.SetAttribute("width", "256");
            __image.SetAttribute("height", "32");
            
            XmlElement __layer = __xmlDoc.CreateElement("layer"); 
            __map.AppendChild(__layer);
            __layer.SetAttribute("name", "name");
            __layer.SetAttribute("width", "20");
            __layer.SetAttribute("height", "15");
            
            XmlElement __data = __xmlDoc.CreateElement("data");
            __layer.AppendChild(__data);
            __data.SetAttribute("encoding", "csv");

            __data.InnerText = Print();

            __xmlDoc.Save(path);   
        }
        public void ResetSpace(int w, int h)
        {
            _width = w;
            _height = h;
            _space = new int[w, h];
            if (_space != null)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _space[x, y] = _wall;
                    }
                }
            }

            Produce();
        }

        public string Print()
        {
            string __result = string.Empty;
            if (_space != null) {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        __result += _space[x, y].ToString() + ",";
                    }
                    __result += "\n";
                }
            }
            return __result.Substring(0, __result.Length - 2) + "\n";
        }

        bool Addcheckpiont(int x, int y)
        {
            if (_space == null || x < 0 || y < 0 || x >= _space.GetLength(0) || y >= _space.GetLength(1))
            {
                return false;
            }

            _checkpionts.Add(new CVector2i(x, y));
            return true;
        }

        void AddEntryAndExit()
        {
            int __entry = 1;
            int __exit = 5;

            _entries = new CVector2i[1];
            _exits = new CVector2i[1];
            CVector2i __entryXY = new CVector2i(_random.Next(_width), _random.Next(_height));
            CVector2i __exitXY = new CVector2i(_random.Next(_width), _random.Next(_height));

            while (__entryXY == __exitXY)
                __exitXY = new CVector2i(_random.Next(_width), _random.Next(_height));

            _space[__entryXY._X, __entryXY._Y] = __entry;
            _space[__exitXY._X, __exitXY._Y] = __exit;
            _entries[0] = __entryXY;
            _exits[0] = __exitXY;
        }

        void FindPath()
        {
            int __exit = 5;
            CVector2i __s = _entries[0] - _exits[0];
            //int __minStep = Math.Abs(__s._X) + Math.Abs(__s._Y);
            //int __maxExtendStep = _width * _height - __minStep;
            while (__s._X != 0 || __s._Y != 0)
            {
                if (__s._X != 0 && __s._Y != 0)
                {
                    if (_random.Next(2) == 0)
                        __s._X += __s._X < 0 ? 1 : -1;
                    else
                        __s._Y += __s._Y < 0 ? 1 : -1;
                }
                else
                {
                    if (__s._X == 0)
                        __s._Y += __s._Y < 0 ? 1 : -1;
                    else
                        __s._X += __s._X < 0 ? 1 : -1;
                }
                _path.Add(_exits[0] + __s);
                //_space[_exits[0]._X + __s._X, _exits[0]._Y + __s._Y] = 0;
            }

            if (_path.Count > 0)
                _path.RemoveAt(_path.Count - 1);
            //_space[_exits[0]._X, _exits[0]._Y] = __exit;
        }

        void RhythmGeneration()
        { 
            //1cell == 0.2sec
            int __jianGe = 5;
            int pianduan = _path.Count / __jianGe;
            CMove sda = new CMove(1, 2);
        }

        void SetObstacles()
        { 
        }

        void Produce()
        {
            AddEntryAndExit();
            FindPath();
        }
    }
}
