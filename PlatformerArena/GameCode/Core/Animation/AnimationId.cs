using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animation
{
    public readonly struct AnimationId
    {
        private readonly string _id;
        public AnimationId(string id) => _id = id;
        public override string ToString() => _id;
    }
}

