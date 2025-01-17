using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class Scene
    {
        public delegate void CallMethod();
        public CallMethod Update;
        public CallMethod Draw;
        public Scene(CallMethod update, CallMethod draw)
        { Update = update; Draw = draw; }
    }

}
