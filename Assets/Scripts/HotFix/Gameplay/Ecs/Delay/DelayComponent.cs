﻿using Saro.Entities;

namespace Tetris
{
    public struct DelayComponent : IEcsComponent
    {
        public float delay;

        public override string ToString()
        {
            return "delay:" + delay.ToString();
        }
    }
}