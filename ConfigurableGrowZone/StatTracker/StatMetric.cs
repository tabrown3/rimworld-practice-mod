﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigurableGrowZone
{
    public abstract class StatMetric
    {
        public readonly string Key;
        public readonly string Name;
        public readonly string Unit;
        public readonly TimeDomain Domain;

        public event EventHandler<DataPointEventArgs> ValuePushed;

        protected readonly IPullable<float> source;


        public StatMetric(string key, string name, IPullable<float> source, string unit, TimeDomain domain)
        {
            this.Key = key;
            this.Name = name;
            this.Unit = unit;
            this.Domain = domain;

            this.source = source;
        }

        public abstract void Tick(int gameTick);

        protected virtual bool ShouldPushValue(int gameTick)
        {
            return Domain.IsResolutionBoundary(gameTick); // should digest on the last tick of the period
        }

        protected void PushValue(int gameTick, float value)
        {
            ValuePushed.Invoke(this, new DataPointEventArgs(new DataPoint(gameTick, value)));
        }
    }
}
